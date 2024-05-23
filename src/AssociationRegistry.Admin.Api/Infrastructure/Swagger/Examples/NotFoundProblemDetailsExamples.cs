namespace AssociationRegistry.Admin.Api.Infrastructure.Swagger.Examples;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;

public class NotFoundProblemDetailsExamples : IExamplesProvider<ProblemDetails>
{
    private readonly ProblemDetailsHelper _helper;
    private readonly IHttpContextAccessor _contextAccessor;

    public NotFoundProblemDetailsExamples(ProblemDetailsHelper helper, IHttpContextAccessor contextAccessor)
    {
        _helper = helper;
        _contextAccessor = contextAccessor;
    }

    public ProblemDetails GetExamples()
        => new()
        {
            HttpStatus = StatusCodes.Status404NotFound,
            Title = ProblemDetails.DefaultTitle,
            Detail = "De gevraagde vereniging werd niet gevonden.",
            ProblemTypeUri = "urn:associationregistry.admin.api:validation",
            ProblemInstanceUri = $"{_helper.GetInstanceBaseUri(_contextAccessor.HttpContext)}/{ProblemDetails.GetProblemNumber()}",
        };
}
