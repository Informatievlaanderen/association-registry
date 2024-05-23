namespace AssociationRegistry.Public.Api.Verenigingen;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;

public class ProblemDetailsExamples : IExamplesProvider<ProblemDetails>
{
    private readonly ProblemDetailsHelper _helper;
    private readonly IHttpContextAccessor _contextAccessor;

    public ProblemDetailsExamples(ProblemDetailsHelper helper, IHttpContextAccessor contextAccessor)
    {
        _helper = helper;
        _contextAccessor = contextAccessor;
    }

    public ProblemDetails GetExamples()
        => new()
        {
            HttpStatus = StatusCodes.Status400BadRequest,
            Title = ProblemDetails.DefaultTitle,
            Detail = "Beschrijving van het probleem",
            ProblemTypeUri = "urn:associationregistry.public.api:validation",
            ProblemInstanceUri = $"{_helper.GetInstanceBaseUri(_contextAccessor.HttpContext)}/{ProblemDetails.GetProblemNumber()}",
        };
}
