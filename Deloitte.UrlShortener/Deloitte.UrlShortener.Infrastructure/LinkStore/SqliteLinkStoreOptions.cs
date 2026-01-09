namespace Deloitte.UrlShortener.Infrastructure.LinkStore;

public sealed class SqliteLinkStoreOptions
{
    public const string SectionName = "LinkStore:Sqlite";

    public string? ConnectionString { get; init; }

    /// <summary>
    /// Optional path to the SQLite file. If set and ConnectionString is not provided,
    /// a default connection string will be built as "Data Source={FilePath}".
    /// </summary>
    public string? FilePath { get; init; }
}
