namespace Deloitte.UrlShortener.Domain;

/// <summary>
/// Domain entity representing a mapping of a short code to a destination URL.
/// Pure data + minimal invariants; no IO and no framework dependencies.
/// </summary>
public sealed record Link
{
    public string Code { get; }
    public string Destination { get; }

    public Link(string code, string destination)
    {
        Code = NormalizeAndValidateCode(code);
        Destination = NormalizeDestination(destination);
    }

    private static string NormalizeAndValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Short code must not be empty.", nameof(code));

        var normalized = code.Trim();

        return normalized;
    }

    private static string NormalizeDestination(string destination)
    {
        if (string.IsNullOrWhiteSpace(destination))
            throw new ArgumentException("Destination must not be empty.", nameof(destination));

        return destination.Trim();
    }
}
