namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

using System.Diagnostics;

public class CustomHeadersActivityMiddleware
{
    private readonly RequestDelegate _next;

    public CustomHeadersActivityMiddleware(RequestDelegate next)
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

        if (context.Request.Headers.TryGetValue(WellknownHeaderNames.CorrelationId, out var correlationId))
        {
            var currentActivity = Activity.Current;

            if (currentActivity != null)
                currentActivity.SetTag(key: "vr.correlationid", correlationId.ToString());
        }

        if (context.Request.Headers.TryGetValue(WellknownHeaderNames.BevestigingsToken, out var bevestigingsToken))
        {
            var currentActivity = Activity.Current;

            if (currentActivity != null)
                currentActivity.SetTag(key: "vr.bevestigingstoken", bevestigingsToken.ToString());
        }

        await _next(context);
    }
}
