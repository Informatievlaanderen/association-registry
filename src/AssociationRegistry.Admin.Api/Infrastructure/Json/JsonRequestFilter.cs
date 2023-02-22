namespace AssociationRegistry.Admin.Api.Infrastructure.Json;

using System.IO;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using JasperFx.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

public class JsonRequestFilter : IAsyncActionFilter
{
    private static readonly JsonLoadSettings JsonLoadSettings = new() { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error };

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
        var body = await context.HttpContext.Request.Body.ReadAllTextAsync();
        if (!string.IsNullOrWhiteSpace(body))
        {
            ThrowifInvalidJson(body);
        }

        context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
        await next();
    }

    private static void ThrowifInvalidJson(string json)
    {
        try
        {
            JObject.Parse(json, JsonLoadSettings);
        }
        catch (JsonReaderException e)
        {
            throw new CouldNotParseRequestException();
        }
    }
}
