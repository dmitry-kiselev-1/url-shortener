using Deloitte.UrlShortener.Domain;

namespace Deloitte.UrlShortener.Application.Abstractions;

/// <summary>
/// Port for retrieving short-link mappings.
/// Infrastructure provides the implementation (file, DB, etc.).
/// </summary>
public interface IShortLinkStore
{
    /// <summary>
    /// Returns a short-link mapping by its code, or <c>null</c> if not found.
    /// </summary>
    ValueTask<ShortLink?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}

