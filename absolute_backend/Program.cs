using Application.Validators;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Application.Validators.Product;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

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
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();

    builder.Services
        .AddFluentValidationAutoValidation()
        .AddFluentValidationClientsideAdapters();

    builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<ProductUpdateValidator>();

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