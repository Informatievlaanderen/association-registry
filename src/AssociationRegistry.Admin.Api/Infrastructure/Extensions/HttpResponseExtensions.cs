namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

public static class HttpResponseExtensions
{
    public static async Task<IActionResult> WriteNotFoundProblemDetailsAsync(
        this HttpResponse response,
        ProblemDetailsHelper problemDetailsHelper,
        string? message = null)
    {
        message ??= ValidationMessages.Status404NotFound;
        await response.WriteProblemDetailsAsync(problemDetailsHelper, message, StatusCodes.Status404NotFound);

        return new EmptyResult();
    }

    public static async Task WriteProblemDetailsAsync(
        this HttpResponse response,
        ProblemDetailsHelper problemDetailsHelper,
        string problemDetailsMessage,
        int statusCode = StatusCodes.Status400BadRequest)
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
                    ProblemInstanceUri =
                        $"{problemDetailsHelper.GetInstanceBaseUri(response.HttpContext)}/{ProblemDetails.GetProblemNumber()}",
                },
                JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi()));
    }
}
