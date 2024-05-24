namespace AssociationRegistry.Public.Api.Verenigingen;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Infrastructure.ConfigurationBindings;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;

public class ProblemDetailsExamples : IExamplesProvider<ProblemDetails>
{
    private readonly ProblemDetailsHelper _helper;
    private readonly AppSettings _appSettings;

    public ProblemDetailsExamples(ProblemDetailsHelper helper, AppSettings appSettings)
    {
        _helper = helper;
        _appSettings = appSettings;

    }

    public ProblemDetails GetExamples()
        => new()
        {
            HttpStatus = StatusCodes.Status400BadRequest,
            Title = ProblemDetails.DefaultTitle,
            Detail = "Beschrijving van het probleem",
            ProblemTypeUri = "urn:associationregistry.public.api:validation",
            ProblemInstanceUri = $"{_appSettings.BaseUrl}/v1/foutmeldingen/{ProblemDetails.GetProblemNumber()}",
        };
}
