using Deloitte.UrlShortener.Application.Resolve;
using Microsoft.Extensions.DependencyInjection;

namespace Deloitte.UrlShortener.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUrlShortenerApplication(this IServiceCollection services)
    {
        services.AddScoped<ResolveShortCodeService>();
        return services;
    }
}

