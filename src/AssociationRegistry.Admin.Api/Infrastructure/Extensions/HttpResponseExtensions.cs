namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;

public static class HttpResponseExtensions
{
    public static async Task WriteProblemDetailsAsync(this HttpResponse response, ProblemDetailsHelper problemDetailsHelper, string problemDetailsMessage, int statusCode = StatusCodes.Status400BadRequest)
    {
        response.StatusCode = statusCode;
        await response.WriteAsync(
            JsonConvert.SerializeObject(
                new ProblemDetails
                {
                    HttpStatus = statusCode,
                    Title = ProblemDetails.DefaultTitle,
                    Detail = problemDetailsMessage,
                    ProblemTypeUri = "urn:associationregistry.admin.api:validation",
                    ProblemInstanceUri = $"{problemDetailsHelper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}",
                },
                JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi()));
    }
}
