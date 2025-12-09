
using Application.Validators;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Application.Validators.Product;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using Application.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Application.Filters;
using StackExchange.Redis;
using Application.Abstractions;
using Infrastructure.Auth;
using System.Text;
using Microsoft.IdentityModel.Tokens;


try
{
    Log.Logger = new LoggerConfiguration()
     .MinimumLevel.Information()
     .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
     .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
     .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
     .MinimumLevel.Override("System", LogEventLevel.Warning)
     .Enrich.FromLogContext()
     .WriteTo.Console()
     .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
     {
         AutoRegisterTemplate = true,
         IndexFormat = $"absolute-logs-{DateTime.UtcNow:yyyy-MM}"
     })
     .CreateLogger();

    Log.Information("Starting application...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    builder.Services.AddAutoMapper(cfg =>
    {
        cfg.AddProfile<ProductProfile>();
        cfg.AddProfile<TitleProfile>();
    });

    builder.Services.AddSwaggerGen();

    builder.Services.AddControllers(options =>
{
    options.Filters.Add<LoggingActionFilter>();
});

    builder.Services
        .AddFluentValidationAutoValidation()
        .AddFluentValidationClientsideAdapters();

    builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<ProductUpdateValidator>();

    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        var config = builder.Configuration["Redis:Connection"] ?? throw new Exception("Redis connection string is not configured.");
        return ConnectionMultiplexer.Connect(config);
    });

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var response = new Application.Exceptions.ErrorResponse
            {
                StatusCode = 400,
                Message = "Validation failed",
                Details = errors
            };

            return new BadRequestObjectResult(response);
        };
    });
    // 1) Options bind
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

    // 2) TokenService kaydı
    builder.Services.AddScoped<ITokenService, TokenService>();

    // 3) JwtOptions değerlerini al (validation için)
    var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
    var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

    // 4) Authentication pipeline
    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // Dev’de böyle
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    });

#pragma warning disable CS0612
    builder.Services.AddApplicationServices().AddInfrastructureServices(builder.Configuration);
#pragma warning restore CS0612

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseMiddleware<Infrastructure.Middleware.ExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}