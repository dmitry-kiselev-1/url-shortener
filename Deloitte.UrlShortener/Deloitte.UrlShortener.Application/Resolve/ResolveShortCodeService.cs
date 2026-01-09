using Deloitte.UrlShortener.Application.Abstractions;

namespace Deloitte.UrlShortener.Application.Resolve;

/// <summary>
/// Use-case service: resolves a short code into a validated destination URL.
/// </summary>
public sealed class ResolveShortCodeService
{
    private readonly IShortLinkStore _store;

    public ResolveShortCodeService(IShortLinkStore store)
    {
        _store = store;
    }

    public async ValueTask<ResolveShortCodeResult> ResolveAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return ResolveShortCodeResult.NotFound();

        var shortLink = await _store.GetByCodeAsync(code.Trim(), cancellationToken);
        if (shortLink is null)
            return ResolveShortCodeResult.NotFound();

        if (!Uri.TryCreate(shortLink.Destination, UriKind.Absolute, out var uri))
            return ResolveShortCodeResult.InvalidDestination("Destination URL is not a valid absolute URL.");

        if (uri.Scheme is not ("http" or "https"))
            return ResolveShortCodeResult.InvalidDestination("Destination URL must use http or https.");

        return ResolveShortCodeResult.Success(uri);
    }
}
