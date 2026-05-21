namespace AssociationRegistry.OpenTelemetry.Middleware;

using System.Text.RegularExpressions;
using Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

/// <summary>
///     Registreert per request op de search- en detail-endpoints van /verenigingen
///     of de 'VR-Api-Version' header aanwezig is, en welke waarde.
///     Enkel de header wordt ondersteund; query-string wordt niet langer gepromoot.
/// </summary>
public class ApiVersionUsageMiddleware
{
    private const string HeaderName = "VR-Api-Version";

    // /verenigingen/zoeken  of  /verenigingen/{vCode}
    // Eventueel voorafgegaan door een version-segment (/v1/, /v2/).
    private static readonly Regex ZoekenPath = new(
        @"^/(?:v\d+(?:\.\d+)?/)?verenigingen/zoeken/?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex DetailPath = new(
        @"^/(?:v\d+(?:\.\d+)?/)?verenigingen/[^/]+/?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private readonly RequestDelegate _next;
    private readonly string _api;

    public ApiVersionUsageMiddleware(RequestDelegate next, string api)
    {
        _next = next;
        _api = api;
    }

    public Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsGet(context.Request.Method))
        {
            var path = context.Request.Path.Value ?? string.Empty;
            var endpoint =
                ZoekenPath.IsMatch(path) ? "search"
                : DetailPath.IsMatch(path) ? "detail"
                : null;

            if (endpoint is not null)
                Record(context, endpoint);
        }

        return _next(context);
    }

    private void Record(HttpContext context, string endpoint)
    {
        var hasHeader = context.Request.Headers.TryGetValue(HeaderName, out var headerValue);

        if (hasHeader)
        {
            ApiVersionMetrics.Record(_api, endpoint, headerValue.ToString(), ApiVersionMetrics.Sources.Header);
            return;
        }

        ApiVersionMetrics.Record(_api, endpoint, rawVersion: null, ApiVersionMetrics.Sources.None);
    }
}

public static class ApiVersionUsageMiddlewareExtensions
{
    public static IApplicationBuilder UseApiVersionUsageMetrics(this IApplicationBuilder app, string api) =>
        app.UseMiddleware<ApiVersionUsageMiddleware>(api);
}
