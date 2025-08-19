namespace AssociationRegistry.Public.Api.Infrastructure.Middleware;

using System.Diagnostics;
using Serilog.Context;

public class ApiKeyEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    private static string ApiKeyName => WellknownHeaderNames.ApiKey;

    public ApiKeyEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? apiKey = null;

        if (context.Request.Headers.TryGetValue(ApiKeyName, out var headerValue))
        {
            apiKey = headerValue.ToString();
        }
        else if (context.Request.Query.TryGetValue(ApiKeyName, out var queryValue))
        {
            apiKey = queryValue.ToString();
        }

        if (!string.IsNullOrEmpty(apiKey))
        {
            var currentActivity = Activity.Current;
            if (currentActivity != null)
            {
                currentActivity.SetTag("vr.api_key", apiKey);
            }

            using (LogContext.PushProperty("ApiKey", apiKey))
            {
                await _next(context);
            }
        }
        else
        {
            await _next(context);
        }
    }
}