using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PrintZPL.Host;
using Xunit;

namespace PrintZPL.Tests;

public sealed class ApiKeyMiddlewareTests
{
    private const string ApiKey = "test-api-key-with-at-least-thirty-two-characters";

    [Fact]
    public async Task InvokeAsync_RejectsMissingApiKey()
    {
        var context = new DefaultHttpContext();
        var middleware = CreateMiddleware(ApiKey, _ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_FailsClosedWhenKeyIsNotConfigured()
    {
        var context = new DefaultHttpContext();
        var middleware = CreateMiddleware(null, _ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_AllowsMatchingApiKey()
    {
        var nextCalled = false;
        var context = new DefaultHttpContext();
        context.Request.Headers["X-API-Key"] = ApiKey;
        var middleware = CreateMiddleware(ApiKey, _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
    }

    private static ApiKeyMiddleware CreateMiddleware(string? apiKey, RequestDelegate next)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Security:ApiKey"] = apiKey })
            .Build();
        return new ApiKeyMiddleware(next, configuration);
    }
}
