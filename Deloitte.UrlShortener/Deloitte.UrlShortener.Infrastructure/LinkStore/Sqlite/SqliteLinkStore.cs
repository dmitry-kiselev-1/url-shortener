using System.Data;
using Deloitte.UrlShortener.Application.Interfaces;
using Deloitte.UrlShortener.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deloitte.UrlShortener.Infrastructure.LinkStore.Sqlite;

/// <summary>
/// SQLite-backed implementation of <see cref="ILinkStore"/>.
/// Uses a single table "links" with unique short codes.
/// On first run (empty table) seeds a fixed set of demo links.
/// </summary>
public sealed class SqliteLinkStore : ILinkStore
{
    private const string TableName = "links";
    private readonly string _connectionString;
    private readonly ILogger<SqliteLinkStore> _logger;

    public SqliteLinkStore(
        IOptions<SqliteLinkStoreOptions> options,
        ILogger<SqliteLinkStore> logger)
    {
        _logger = logger;

        var opts = options.Value ?? throw new InvalidOperationException("SqliteLinkStore options must be configured.");

        if (!string.IsNullOrWhiteSpace(opts.ConnectionString))
        {
            _connectionString = opts.ConnectionString;
        }
        else if (!string.IsNullOrWhiteSpace(opts.FilePath))
        {
            var path = opts.FilePath;
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(AppContext.BaseDirectory, path);
            }

            _connectionString = $"Data Source={path}";
        }
        else
        {
            throw new InvalidOperationException("Either LinkStore:Sqlite:ConnectionString or LinkStore:Sqlite:FilePath must be configured when using SqliteLinkStore.");
        }

        EnsureSchema();
        SeedDemoDataIfEmpty();
    }

    public async ValueTask<Link?> SearchAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var normalized = code.Trim();

        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            await using var command = connection.CreateCommand();
            command.CommandText = $"SELECT code, url FROM {TableName} WHERE code = @code COLLATE NOCASE LIMIT 1;";
            command.Parameters.Add(new SqliteParameter("@code", DbType.String) { Value = normalized });

            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                return null;

            var dbCode = reader.GetString(0);
            var url = reader.GetString(1);

            try
            {
                return new Link(dbCode, url);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to materialize Link from SQLite row for code {Code}", dbCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while querying SQLite link store for code {Code}", normalized);
            throw;
        }
    }

    private void EnsureSchema()
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $@"CREATE TABLE IF NOT EXISTS {TableName} (
    id    INTEGER PRIMARY KEY AUTOINCREMENT,
    code  TEXT NOT NULL UNIQUE COLLATE NOCASE,
    url   TEXT NOT NULL
);";
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure SQLite schema for table {TableName}", TableName);
            throw;
        }
    }

    private void SeedDemoDataIfEmpty()
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using (var countCommand = connection.CreateCommand())
            {
                countCommand.CommandText = $"SELECT COUNT(*) FROM {TableName};";
                var count = (long)(countCommand.ExecuteScalar() ?? 0L);
                if (count > 0)
                {
                    return; // Already has data; do not overwrite.
                }
            }

            var demoLinks = new[]
            {
                new Link("link1", "https://www.google.com"),
                new Link("link2", "https://www.amazon.com"),
                new Link("link3", "https://example.com/page1/long/url"),
                new Link("link4", "https://another.com/this/is/a/very/long/one")
            };

            using var transaction = connection.BeginTransaction();
            using var insertCommand = connection.CreateCommand();
            insertCommand.Transaction = transaction;
            insertCommand.CommandText = $"INSERT OR IGNORE INTO {TableName} (code, url) VALUES (@code, @url);";

            var codeParam = insertCommand.CreateParameter();
            codeParam.ParameterName = "@code";
            insertCommand.Parameters.Add(codeParam);

            var urlParam = insertCommand.CreateParameter();
            urlParam.ParameterName = "@url";
            insertCommand.Parameters.Add(urlParam);

            var inserted = 0;
            foreach (var link in demoLinks)
            {
                codeParam.Value = link.Code;
                urlParam.Value = link.Destination;
                inserted += insertCommand.ExecuteNonQuery();
            }

            transaction.Commit();
            _logger.LogInformation("Seeded {Count} demo short link mappings into SQLite store.", inserted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed demo data into SQLite store.");
        }
    }
}
