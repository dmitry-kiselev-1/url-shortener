namespace Deloitte.UrlShortener.Domain;

/// <summary>
/// Domain entity representing a mapping of a short code to a destination URL.
/// Pure data + minimal invariants; no IO and no framework dependencies.
/// </summary>
public sealed record ShortLink
{
    public string Code { get; }
    public string Destination { get; }

    public ShortLink(string code, string destination)
    {
        Code = NormalizeAndValidateCode(code);
        Destination = NormalizeDestination(destination);
    }

    private static string NormalizeAndValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Short code must not be empty.", nameof(code));

        var normalized = code.Trim();

        // Keep invariants intentionally simple. No regex/format rules yet.
        // The API route is /{code}, so '/' cannot be part of a single segment anyway.
        return normalized;
    }

    private static string NormalizeDestination(string destination)
    {
        if (string.IsNullOrWhiteSpace(destination))
            throw new ArgumentException("Destination must not be empty.", nameof(destination));

        return destination.Trim();
    }
}
