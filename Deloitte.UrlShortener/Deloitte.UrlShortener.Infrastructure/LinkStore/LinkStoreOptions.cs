namespace Deloitte.UrlShortener.Infrastructure.LinkStore;

public sealed class LinkStoreOptions
{
    public const string SectionName = "LinkStore";

    public string FilePath { get; init; } = "links.txt";
}

