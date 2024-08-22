namespace AssociationRegistry.Admin.Api.Infrastructure.Json;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using JasperFx.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JsonRequestFilter : IAsyncActionFilter
{
    private static readonly JsonLoadSettings JsonLoadSettings = new()
        { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error };

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.Request.Body.Seek(offset: 0, SeekOrigin.Begin);
        var body = await context.HttpContext.Request.Body.ReadAllTextAsync();
        if (!string.IsNullOrWhiteSpace(body)) ThrowifInvalidJson(body);

        context.HttpContext.Request.Body.Seek(offset: 0, SeekOrigin.Begin);
        await next();
    }

    private static void ThrowifInvalidJson(string json)
    {
        JObject.Parse(json, JsonLoadSettings);
    }
}

public class JsonReaderExceptionHandler : DefaultExceptionHandler<JsonReaderException>
{
    private readonly ProblemDetailsHelper _problemDetailsHelper;

    public JsonReaderExceptionHandler(ProblemDetailsHelper problemDetailsHelper)
    {
        _problemDetailsHelper = problemDetailsHelper;
    }

    protected override ProblemDetails GetApiProblemFor(HttpContext context, JsonReaderException exception)
        => new()
        {
            HttpStatus = StatusCodes.Status400BadRequest,
            Title = ProblemDetails.DefaultTitle,
            Detail = "Request body bevat een ongeldig JSON formaat.",
            ProblemTypeUri = _problemDetailsHelper.GetExceptionTypeUriFor(exception),
            ProblemInstanceUri = $"{_problemDetailsHelper.GetInstanceBaseUri(context)}/{ProblemDetails.GetProblemNumber()}",
        };
}
