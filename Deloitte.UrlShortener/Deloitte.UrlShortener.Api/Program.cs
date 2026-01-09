using Deloitte.UrlShortener.Application.DI;
using Deloitte.UrlShortener.Application.Services;
using Deloitte.UrlShortener.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
   .WithName("HealthCheck")
   .WithSummary("Simple health check endpoint to verify that the API is running");

// Redirect endpoint
app.MapGet("/{code}", async (string code, SearchService searchService, CancellationToken ct) =>
{
    var result = await searchService.SearchAsync(code, ct);

    if (!result.Found)
        return Results.NotFound();

    if (!result.IsValidDestination || result.Destination is null)
        return Results.BadRequest(new { error = result.Error ?? "Invalid destination URL." });

    // Permanent Redirect: HTTP 308
    return Results.Redirect(result.Destination.ToString(), permanent: true);
})
.WithName("ResolveShortCode")
.WithSummary("Resolves a short code and permanently redirects to the destination URL")
.Produces(StatusCodes.Status308PermanentRedirect)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status400BadRequest);

app.Run();
