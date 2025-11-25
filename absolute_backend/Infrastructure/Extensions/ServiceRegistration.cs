using Infrastructure.Data;
using Infrastructure.Messaging.Sagas;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Data.UnitOfWork;
using Infrastructure.Data.Repositories.Concrete;
using Infrastructure.Data.Repositories.Abstract;
using Application.Filters;
using Infrastructure.Caching;
using Application.Abstractions.Caching;


namespace Infrastructure.Extensions;

public static class ServiceRegistration
{
    // Application services
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<Application.Services.TitleService>();
        services.AddScoped<Application.Services.GenreService>();
        services.AddScoped<Application.Services.OrderService>();
        services.AddScoped<Application.Services.ProductService>();
        services.AddScoped<Application.Services.OrderHistoryService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<LoggingActionFilter>();
        services.AddScoped<ICacheService, RedisCacheService>();
        return services;
    }

    // Infrastructure services
    [Obsolete]
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        // TMDb HttpClient
        services.AddHttpClient<TmdbService>(c =>
        {
            c.BaseAddress = new Uri("https://api.themoviedb.org/3/");
        });

        // MassTransit + RabbitMQ + Saga
        services.AddMassTransit(x =>
        {
            // Consumers
            x.AddConsumer<PaymentConsumer>();
            x.AddConsumer<OrderHistoryConsumer>();


            // Saga State Machine
            x.AddSagaStateMachine<OrderSaga, OrderState>()
         .EntityFrameworkRepository(r =>
         {
             r.ConcurrencyMode = ConcurrencyMode.Optimistic; // default, no issue
             r.ExistingDbContext<AppDbContext>();
             r.AddDbContext<DbContext, AppDbContext>((provider, builder) =>
             {
                 builder.UseSqlServer(
                     config.GetConnectionString("DefaultConnection"),
                     m => m.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                 );
             });
         });


            // RabbitMQ
            x.SetKebabCaseEndpointNameFormatter();
            x.UsingRabbitMq((context, cfg) =>
            {
                // appsettings.json or secrets 
                var host = config["RabbitMQ:Host"] ?? "localhost";
                cfg.Host("localhost", "/", h =>
 {
     h.Username(config["RabbitMQ:Username"] ?? "guest");
     h.Password(config["RabbitMQ:Password"] ?? "guest");
 });
                cfg.ReceiveEndpoint("order-history", e =>
                {
                    e.ConfigureConsumer<OrderHistoryConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        // Hosted service bus
        services.AddMassTransitHostedService();

        return services;
    }
}