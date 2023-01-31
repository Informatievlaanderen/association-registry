namespace AssociationRegistry.Admin.Api.Infrastructure;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Microsoft.AspNetCore.Http;
using Verenigingen.Registreer;

public class CouldNotParseRequestExceptionHandler : DefaultExceptionHandler<CouldNotParseRequestException>
{
    private readonly ProblemDetailsHelper _problemDetailsHelper;

    public CouldNotParseRequestExceptionHandler(ProblemDetailsHelper problemDetailsHelper)
        => _problemDetailsHelper = problemDetailsHelper;

    protected override ProblemDetails GetApiProblemFor(CouldNotParseRequestException exception) =>
        new ValidationProblemDetails
        {
            HttpStatus = StatusCodes.Status400BadRequest,
            Title = ProblemDetails.DefaultTitle,
            Detail = exception.Message,
            ProblemTypeUri = _problemDetailsHelper.GetExceptionTypeUriFor(exception),
            ProblemInstanceUri = $"{_problemDetailsHelper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}",
        };
}
