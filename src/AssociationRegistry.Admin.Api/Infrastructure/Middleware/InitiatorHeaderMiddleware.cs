namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;

public class InitiatorHeaderMiddleware
{
    private readonly string[] _methodsRequiringInitiator =
    {
        HttpMethods.Delete,
        //HttpMethods.Patch,
        //HttpMethods.Post,
        //HttpMethods.Put,
    };

    private readonly RequestDelegate _next;

    public InitiatorHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ProblemDetailsHelper problemDetailsHelper)
    {
        var initiatorHeaderNotRequired = !_methodsRequiringInitiator.Contains(context.Request.Method);
        var initiatorHeaderMissing = !context.Request.Headers.ContainsKey(WellknownHeaderNames.Initiator);
        var initiatorHeaderEmpty = string.IsNullOrWhiteSpace(context.Request.Headers[WellknownHeaderNames.Initiator]);

        if (initiatorHeaderNotRequired)
            await _next(context);
        else if (initiatorHeaderMissing)
            await WriteProblemDetails(context, problemDetailsHelper, "Initiator is verplicht.");
        else if (initiatorHeaderEmpty)
            await WriteProblemDetails(context, problemDetailsHelper, "Initiator mag niet leeg zijn.");
        else
            await _next(context);
    }

    private static async Task WriteProblemDetails(HttpContext context, ProblemDetailsHelper problemDetailsHelper, string problemDetailsMessage)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync(
            JsonConvert.SerializeObject(new ProblemDetails
            {
                HttpStatus = StatusCodes.Status400BadRequest,
                Title = ProblemDetails.DefaultTitle,
                Detail = problemDetailsMessage,
                ProblemTypeUri = "urn:associationregistry.admin.api:validation",
                ProblemInstanceUri = $"{problemDetailsHelper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}",
            }, JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi()));
    }
}
