using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Deloitte.UrlShortener.Api.Tests.e2e;

public class RedirectEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = CreateHttpsClient(factory);

    private static HttpClient CreateHttpsClient(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient(new()
        {
            // for redirect tests, we want to see the 3xx response, not follow it
            AllowAutoRedirect = false
        });

        client.BaseAddress = new Uri("https://localhost:8081/");
        return client;
    }

    // Covered scenarios:
    // - Successful redirect for existing codes (link1, link3).
    // - 404 for non-existing code.
    // - Health endpoint returns 200/"Healthy".
    
    // Missing important scenarios (not yet covered):
    // - Code exists but destination is invalid -> 400 BadRequest.
    // - Case-insensitive codes (e.g. LINK1, Link1) map to the same destination.

    [Fact]
    public async Task Health_Returns_Healthy()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", body);
    }    
    
    [Theory]
    [InlineData("link1", "https://www.google.com")]
    [InlineData("link3", "https://example.com/page1/long/url")]
    public async Task ExistingCode_Returns_Redirect_To_Expected_Location(string code, string expectedLocation)
    {
        var response = await _client.GetAsync($"/{code}");

        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);

        var actualLocation = response.Headers.Location?.ToString().TrimEnd('/');
        var expected = expectedLocation.TrimEnd('/');
        Assert.Equal(expected, actualLocation);
    }

    [Fact]
    public async Task NonExistingCode_Returns_404()
    {
        var response = await _client.GetAsync("/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
