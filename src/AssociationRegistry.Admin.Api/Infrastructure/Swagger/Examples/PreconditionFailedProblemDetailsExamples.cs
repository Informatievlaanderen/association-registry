namespace AssociationRegistry.Admin.Api.Infrastructure.Swagger.Examples;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;

public class PreconditionFailedProblemDetailsExamples : IExamplesProvider<ProblemDetails>
{
    private readonly ProblemDetailsHelper _helper;

    public PreconditionFailedProblemDetailsExamples(ProblemDetailsHelper helper)
    {
        _helper = helper;
    }
    public ProblemDetails GetExamples()
        => new()
        {
            HttpStatus = StatusCodes.Status412PreconditionFailed,
            Title = ProblemDetails.DefaultTitle,
            Detail = "De gevraagde vereniging heeft niet de verwachte sequentiewaarde.",
            ProblemTypeUri = "urn:associationregistry.admin.api:validation",
            ProblemInstanceUri = $"{_helper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}",
        };
}
