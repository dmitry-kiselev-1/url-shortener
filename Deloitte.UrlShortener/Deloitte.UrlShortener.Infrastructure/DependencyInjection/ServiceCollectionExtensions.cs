using Deloitte.UrlShortener.Application.Abstractions;
using Deloitte.UrlShortener.Infrastructure.ShortLinkStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deloitte.UrlShortener.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUrlShortenerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ShortLinkStoreOptions>()
            .Bind(configuration.GetSection(ShortLinkStoreOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.FilePath), "ShortLinkStore:FilePath must be configured.")
            .ValidateOnStart();

        // Singleton: loads file once at startup.
        services.AddSingleton<IShortLinkStore, FileShortLinkStore>();

        return services;
    }
}

