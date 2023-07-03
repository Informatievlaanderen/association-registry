namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    public const string CorrelationIdHeader = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ProblemDetailsHelper helper)
    {
        if (context.Request.Path.HasValue && context.Request.Path.Value.ToLowerInvariant().StartsWith("/v1"))
        {
            var correlationId = GetCorrelationId(context);

            if (correlationId is null)
            {
                await WriteProblemDetails(context, helper, $"{CorrelationIdHeader} is een verplichte header.");
                return;
            }

            AddCorrelationIdHeaderToResponse(context, correlationId.Value);
        }

        await _next(context);
    }

    private static StringValues? GetCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
        {
            return correlationId;
        }

        return null;
    }

    private static void AddCorrelationIdHeaderToResponse(HttpContext context, StringValues correlationId)
    {
        context.Response.OnStarting(
            () =>
            {
                context.Response.Headers.Remove(CorrelationIdHeader);
                context.Response.Headers.Add(CorrelationIdHeader, new[] { correlationId.ToString() });
                return Task.CompletedTask;
            });
    }

    private static async Task WriteProblemDetails(HttpContext context, ProblemDetailsHelper problemDetailsHelper, string problemDetailsMessage)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync(
            JsonConvert.SerializeObject(
                new ProblemDetails
                {
                    HttpStatus = StatusCodes.Status400BadRequest,
                    Title = ProblemDetails.DefaultTitle,
                    Detail = problemDetailsMessage,
                    ProblemTypeUri = "urn:associationregistry.admin.api:validation",
                    ProblemInstanceUri = $"{problemDetailsHelper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}",
                },
                JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi()));
    }
}
