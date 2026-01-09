namespace Deloitte.UrlShortener.Infrastructure.ShortLinkStore;

public sealed class ShortLinkStoreOptions
{
    public const string SectionName = "ShortLinkStore";

    public string FilePath { get; init; } = "shortlinks.txt";
}

