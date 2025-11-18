using Application.Validators;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Application.Validators.Product;
using Serilog;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    Log.Information("Starting application...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

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