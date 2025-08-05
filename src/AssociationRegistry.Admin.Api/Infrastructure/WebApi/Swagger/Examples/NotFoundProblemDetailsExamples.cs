namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Examples;

using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Swashbuckle.AspNetCore.Filters;

public class NotFoundProblemDetailsExamples : IExamplesProvider<ProblemDetails>
{
    private readonly ProblemDetailsHelper _helper;
    private readonly AppSettings _appSettings;

    public NotFoundProblemDetailsExamples(ProblemDetailsHelper helper, AppSettings appSettings)
    {
        _helper = helper;
        _appSettings = appSettings;
    }

    public ProblemDetails GetExamples()
        => new()
        {
            HttpStatus = StatusCodes.Status404NotFound,
            Title = ProblemDetails.DefaultTitle,
            Detail = "De gevraagde vereniging werd niet gevonden.",
            ProblemTypeUri = "urn:associationregistry.admin.api:validation",
            ProblemInstanceUri = $"{_appSettings.BaseUrl}/v1/foutmeldingen/{ProblemDetails.GetProblemNumber()}",
        };
}
