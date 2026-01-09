using System.Collections.Frozen;
using Deloitte.UrlShortener.Application.Abstractions;
using Deloitte.UrlShortener.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deloitte.UrlShortener.Infrastructure.ShortLinkStore;

/// <summary>
/// File-backed store. Reads mappings once at start into memory.
/// Each line format:
///   <code>CODE URL</code>
/// separated by whitespace (space or tab). Lines starting with # are comments.
/// Example:
///   abc https://example.com/page1/long/url
///   short https://another.com/this/is/a/very/long/one
/// </summary>
public sealed class FileShortLinkStore : IShortLinkStore
{
    private readonly FrozenDictionary<string, ShortLink> _byCode;

    public FileShortLinkStore(IOptions<ShortLinkStoreOptions> options, ILogger<FileShortLinkStore> logger)
    {
        var path = options.Value.FilePath;
        if (string.IsNullOrWhiteSpace(path))
            throw new InvalidOperationException("ShortLinkStore:FilePath must be configured.");

        if (!Path.IsPathRooted(path!))
        {
            // Resolve relative path against app base directory (bin output).
            path = Path.Combine(AppContext.BaseDirectory, path);
        }

        if (!File.Exists(path))
            throw new FileNotFoundException("Short link mapping file not found.", path);

        var builder = new Dictionary<string, ShortLink>(StringComparer.OrdinalIgnoreCase);
        var lines = File.ReadAllLines(path);

        for (var i = 0; i < lines.Length; i++)
        {
            var raw = lines[i];
            if (string.IsNullOrWhiteSpace(raw))
                continue;

            var line = raw.Trim();
            if (line.StartsWith('#'))
                continue;

            var parts = line.Split((char[]?)null, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                logger.LogWarning("Invalid mapping line {LineNumber} in {FilePath}: '{Line}'", i + 1, path, raw);
                continue;
            }

            try
            {
                var shortLink = new ShortLink(parts[0], parts[1]);
                if (!builder.TryAdd(shortLink.Code, shortLink))
                {
                    logger.LogWarning("Duplicate short code '{Code}' in {FilePath} (line {LineNumber}). Using first occurrence.", shortLink.Code, path, i + 1);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to parse mapping line {LineNumber} in {FilePath}: '{Line}'", i + 1, path, raw);
            }
        }

        _byCode = builder.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
        logger.LogInformation("Loaded {Count} short link mappings from {FilePath}", _byCode.Count, path);
    }

    public ValueTask<ShortLink?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return ValueTask.FromResult<ShortLink?>(null);

        return ValueTask.FromResult(_byCode.TryGetValue(code.Trim(), out var v) ? v : null);
    }
}
