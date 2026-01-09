namespace Deloitte.UrlShortener.Infrastructure.LinkStore.Cache;

public sealed class CachedFileLinkStoreOptions
{
    public const string SectionName = "Infrastructure:LinkStore:Cache";

    public string FilePath { get; init; } = "links.txt";
}
