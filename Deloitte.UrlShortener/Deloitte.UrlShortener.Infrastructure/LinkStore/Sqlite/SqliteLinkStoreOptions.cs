namespace Deloitte.UrlShortener.Infrastructure.LinkStore.Sqlite;

public sealed class SqliteLinkStoreOptions
{
    public const string SectionName = "Infrastructure:LinkStore:Sqlite";

    public string? ConnectionString { get; init; }

    /// <summary>
    /// Optional path to the SQLite file. If set and ConnectionString is not provided,
    /// a default connection string will be built as "Data Source={FilePath}".
    /// </summary>
    public string? FilePath { get; init; }
}
