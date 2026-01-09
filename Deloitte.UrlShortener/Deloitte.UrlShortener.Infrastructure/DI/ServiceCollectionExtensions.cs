using Deloitte.UrlShortener.Application.Abstractions;
using Deloitte.UrlShortener.Infrastructure.LinkStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deloitte.UrlShortener.Infrastructure.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<LinkStoreOptions>()
            .Bind(configuration.GetSection(LinkStoreOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.FilePath), "LinkStore:FilePath must be configured.")
            .ValidateOnStart();

        services.AddOptions<SqliteLinkStoreOptions>()
            .Bind(configuration.GetSection(SqliteLinkStoreOptions.SectionName));

        // File-based, cached in memory:
        services.AddSingleton<ILinkStore, CachedFileLinkStore>();
        
        // SQLite-based store with demo seeding:
        // services.AddSingleton<ILinkStore, SqliteLinkStore>();

        return services;
    }
}
