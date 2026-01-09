using Deloitte.UrlShortener.Application.DependencyInjection;
using Deloitte.UrlShortener.Application.Resolve;
using Deloitte.UrlShortener.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddUrlShortenerApplication();
builder.Services.AddUrlShortenerInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/{code}", async (string code, ResolveShortCodeService resolver, CancellationToken ct) =>
{
    var result = await resolver.ResolveAsync(code, ct);

    if (!result.Found)
        return Results.NotFound();

    if (!result.IsValidDestination || result.Destination is null)
        return Results.BadRequest(new { error = result.Error ?? "Invalid destination URL." });

    // Permanent Redirect: HTTP 308
    return Results.Redirect(result.Destination.ToString(), permanent: true);
});

app.Run();
