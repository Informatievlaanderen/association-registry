namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Middleware;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;

public class InitiatorHeaderMiddleware
{
    private readonly string[] _methodsNotRequiringInitiator =
    {
        HttpMethods.Options,
        HttpMethods.Head,
    };

    private readonly RequestDelegate _next;

    public InitiatorHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ProblemDetailsHelper problemDetailsHelper, InitiatorProvider initiatorProvider)
    {
        var isV1Route = context.Request.Path.HasValue && context.Request.Path.Value.ToLowerInvariant().StartsWith("/v1");
        var initiatorHeaderNotRequired = _methodsNotRequiringInitiator.Contains(context.Request.Method);
        var initiatorHeaderMissing = !context.Request.Headers.ContainsKey(WellknownHeaderNames.Initiator);
        var initiatorHeaderEmpty = string.IsNullOrWhiteSpace(context.Request.Headers[WellknownHeaderNames.Initiator]);

        if (!isV1Route || initiatorHeaderNotRequired)
        {
            await _next(context);
        }
        else if (initiatorHeaderMissing)
        {
            await context.Response.WriteProblemDetailsAsync(problemDetailsHelper, $"{WellknownHeaderNames.Initiator} is verplicht.");
        }
        else if (initiatorHeaderEmpty)
        {
            await context.Response.WriteProblemDetailsAsync(problemDetailsHelper, $"{WellknownHeaderNames.Initiator} mag niet leeg zijn.");
        }
        else
        {
            initiatorProvider.Value = context.Request.Headers[WellknownHeaderNames.Initiator];
            await _next(context);
        }
    }
}

public interface IInitiatorProvider
{
    string Value { get; set; }
}

public class InitiatorProvider : IInitiatorProvider
{
    public string Value { get; set; } = string.Empty;

    public static implicit operator string(InitiatorProvider initiatorProvider)
        => initiatorProvider.Value;
}
