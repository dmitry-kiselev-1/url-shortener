using Deloitte.UrlShortener.Application.Abstractions;
using Deloitte.UrlShortener.Application.Models;

namespace Deloitte.UrlShortener.Application.Services;

/// <summary>
/// Use-case service: resolves a short code into a validated destination URL.
/// </summary>
public sealed class SearchService(ILinkStore store)
{
    public async ValueTask<SearchResult> SearchAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return SearchResult.NotFound();

        var link = await store.SearchAsync(code.Trim(), cancellationToken);
        if (link is null)
            return SearchResult.NotFound();

        if (!Uri.TryCreate(link.Destination, UriKind.Absolute, out var uri))
            return SearchResult.InvalidDestination("Destination URL is not a valid absolute URL.");

        if (uri.Scheme is not ("http" or "https"))
            return SearchResult.InvalidDestination("Destination URL must use http or https.");

        return SearchResult.Success(uri);
    }
}
