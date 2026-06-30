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
    // /verenigingen/zoeken  of  /verenigingen/{vCode}
    // Eventueel voorafgegaan door een version-segment (/v1/, /v2/).
    // evenals de registratie-endpoints van /feitelijkeverenigingen & /vzer

    private const string HeaderName = "VR-Api-Version";

    private const string RegistrationV1Endpoint = "registration_v1_feitelijkevereniging";
    private const string RegistrationV2Endpoint = "registration_v2_vzer";

    private static readonly Regex ZoekenPath = new(
        @"^/(?:v\d+(?:\.\d+)?/)?verenigingen/zoeken/?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex DetailPath = new(
        @"^/(?:v\d+(?:\.\d+)?/)?verenigingen/[^/]+/?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex FeitelijkeVerenigingenPath = new(
        @"^/(?:v\d+(?:\.\d+)?/)?verenigingen/feitelijkeverenigingen/?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex VzerPath = new(
        @"^/(?:v\d+(?:\.\d+)?/)?verenigingen/vzer/?$",
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
        var endpoint = GetEndpoint(context);

        if (endpoint is not null)
            Record(context, endpoint);

        return _next(context);
    }

    private static string? GetEndpoint(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (HttpMethods.IsGet(context.Request.Method))
        {
            return ZoekenPath.IsMatch(path) ? "search"
                : DetailPath.IsMatch(path) ? "detail"
                : null;
        }

        if (HttpMethods.IsPost(context.Request.Method))
        {
            return FeitelijkeVerenigingenPath.IsMatch(path) ? RegistrationV1Endpoint
                : VzerPath.IsMatch(path) ? RegistrationV2Endpoint
                : null;
        }

        return null;
    }

    private void Record(HttpContext context, string endpoint)
    {
        if (endpoint == RegistrationV1Endpoint)
        {
            ApiVersionMetrics.Record(_api, endpoint, "v1", "path");
            return;
        }

        if (endpoint == RegistrationV2Endpoint)
        {
            ApiVersionMetrics.Record(_api, endpoint, "v2", "path");
            return;
        }

        var hasHeader = context.Request.Headers.TryGetValue(HeaderName, out var headerValue);

        ApiVersionMetrics.Record(
            _api,
            endpoint,
            hasHeader ? headerValue.ToString() : null,
            hasHeader ? ApiVersionMetrics.Sources.Header : ApiVersionMetrics.Sources.None
        );
    }
}

public static class ApiVersionUsageMiddlewareExtensions
{
    public static IApplicationBuilder UseApiVersionUsageMetrics(this IApplicationBuilder app, string api) =>
        app.UseMiddleware<ApiVersionUsageMiddleware>(api);
}
