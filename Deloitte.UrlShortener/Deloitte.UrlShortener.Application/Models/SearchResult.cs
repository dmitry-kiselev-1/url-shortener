namespace Deloitte.UrlShortener.Application.Models;

public sealed record SearchResult
{
    public bool Found { get; init; }
    public bool IsValidDestination { get; init; }
    public Uri? Destination { get; init; }
    public string? Error { get; init; }

    public static SearchResult Success(Uri destination) => new()
    {
        Found = true,
        IsValidDestination = true,
        Destination = destination
    };

    public static SearchResult NotFound() => new()
    {
        Found = false,
        IsValidDestination = false
    };

    public static SearchResult InvalidDestination(string error) => new()
    {
        Found = true,
        IsValidDestination = false,
        Error = error
    };
}

