namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Middleware;

using CommandMiddleware;

/// <summary>
/// The correlation id as it appears on log records: the client-supplied X-Correlation-Id
/// when the request carried a valid one, otherwise the ASP.NET trace identifier so
/// internal-only requests (health, swagger, metrics) remain traceable too.
///
/// The client id wins because it is the only identifier the caller knows — correlating
/// "your operation failed, here is my id" is the whole point of the property.
/// </summary>
internal static class ClientCorrelationId
{
    public static string OrFallbackToTraceIdentifier(ICorrelationIdProvider provider, HttpContext context) =>
        provider.CorrelationId ?? context.TraceIdentifier;
}
