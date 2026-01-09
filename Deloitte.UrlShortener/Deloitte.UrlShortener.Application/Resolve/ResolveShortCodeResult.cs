namespace Deloitte.UrlShortener.Application.Resolve;

public sealed record ResolveShortCodeResult
{
    public bool Found { get; init; }
    public bool IsValidDestination { get; init; }
    public Uri? Destination { get; init; }
    public string? Error { get; init; }

    public static ResolveShortCodeResult Success(Uri destination) => new()
    {
        Found = true,
        IsValidDestination = true,
        Destination = destination
    };

    public static ResolveShortCodeResult NotFound() => new()
    {
        Found = false,
        IsValidDestination = false
    };

    public static ResolveShortCodeResult InvalidDestination(string error) => new()
    {
        Found = true,
        IsValidDestination = false,
        Error = error
    };
}

