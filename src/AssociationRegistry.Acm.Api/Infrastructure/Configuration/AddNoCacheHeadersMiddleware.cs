namespace AssociationRegistry.Acm.Api.Infrastructure.Configuration;

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

/// <summary>
///     Add headers to the response to prevent any caching.
/// </summary>
public class AddNoCacheHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public AddNoCacheHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        context.Response.Headers.Add(HeaderNames.CacheControl, value: "no-store, no-cache, must-revalidate");
        context.Response.Headers.Add(HeaderNames.Pragma, value: "no-cache");
        context.Response.Headers.Add(HeaderNames.Expires, value: "0");

        return _next(context);
    }
}
