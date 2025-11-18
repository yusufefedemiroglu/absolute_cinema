using Application.Validators;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Application.Validators.Product;




var builder = WebApplication.CreateBuilder(args);




builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services
    .AddFluentValidationAutoValidation() // auto server-side validation
    .AddFluentValidationClientsideAdapters(); // for client-side validation

builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateValidator>(); // register validators
builder.Services.AddValidatorsFromAssemblyContaining<ProductUpdateValidator>();

builder.Services.Configure<ApiBehaviorOptions>(options => // automatic model validation response
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


//services
#pragma warning disable CS0612 // Type or member is obsolete
builder.Services.AddApplicationServices().AddInfrastructureServices(builder.Configuration);
#pragma warning restore CS0612 // Type or member is obsolete


var app = builder.Build();

app.UseMiddleware<Infrastructure.Middleware.ExceptionMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

app.Run();

Log.CloseAndFlush();
