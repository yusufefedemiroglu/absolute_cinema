using Infrastructure.Services;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpClient<TmdbService>();
        services.AddScoped<TitleService>();


        return services;
    }
}
