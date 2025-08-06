namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Middleware;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using CommandMiddleware;
using Microsoft.Extensions.Primitives;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ProblemDetailsHelper helper, ICorrelationIdProvider correlationIdProvider)
    {
        if (context.Request.Path.HasValue && context.Request.Path.Value.ToLowerInvariant().StartsWith("/v1"))
        {
            var correlationId = GetCorrelationId(context);

            if (correlationId is null)
            {
                await context.Response.WriteProblemDetailsAsync(helper, $"{WellknownHeaderNames.CorrelationId} is verplicht.");

                return;
            }

            if (!Guid.TryParse(correlationId.Value.ToString(), out _))
            {
                await context.Response.WriteProblemDetailsAsync(
                    helper, $"{WellknownHeaderNames.CorrelationId} moet een geldige GUID zijn.");

                return;
            }

            correlationIdProvider.CorrelationId = correlationId.Value;
            AddCorrelationIdHeaderToResponse(context, correlationId.Value);
        }

        await _next(context);
    }

    private static StringValues? GetCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(WellknownHeaderNames.CorrelationId, out var correlationId))
            return correlationId;

        return null;
    }

    private static void AddCorrelationIdHeaderToResponse(HttpContext context, StringValues correlationId)
    {
        context.Response.OnStarting(
            () =>
            {
                context.Response.Headers.Remove(WellknownHeaderNames.CorrelationId);
                context.Response.Headers.Add(WellknownHeaderNames.CorrelationId, new[] { correlationId.ToString() });

                return Task.CompletedTask;
            });
    }
}
