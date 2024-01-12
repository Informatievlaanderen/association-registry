namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

public class InitiatorActivityMiddleware
{
    private readonly RequestDelegate _next;

    public InitiatorActivityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(WellknownHeaderNames.Initiator, out var headerValue))
        {
            var currentActivity = Activity.Current;

            if (currentActivity != null)
                currentActivity.SetTag(key: "vr.initiator", headerValue.ToString());
        }

        await _next(context);
    }
}
