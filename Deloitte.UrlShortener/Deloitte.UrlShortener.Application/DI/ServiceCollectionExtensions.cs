using Deloitte.UrlShortener.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Deloitte.UrlShortener.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<SearchService>();
        return services;
    }
}

