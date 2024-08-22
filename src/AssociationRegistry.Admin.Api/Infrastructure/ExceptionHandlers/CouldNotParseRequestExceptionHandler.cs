namespace AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;

public class CouldNotParseRequestExceptionHandler : DefaultExceptionHandler<CouldNotParseRequestException>
{
    private readonly ProblemDetailsHelper _problemDetailsHelper;

    public CouldNotParseRequestExceptionHandler(ProblemDetailsHelper problemDetailsHelper)
    {
        _problemDetailsHelper = problemDetailsHelper;
    }

    protected override ProblemDetails GetApiProblemFor(HttpContext context, CouldNotParseRequestException exception)
        => new()
        {
            HttpStatus = StatusCodes.Status400BadRequest,
            Title = ProblemDetails.DefaultTitle,
            Detail = exception.Message,
            ProblemTypeUri = _problemDetailsHelper.GetExceptionTypeUriFor(exception),
            ProblemInstanceUri = $"{_problemDetailsHelper.GetInstanceBaseUri(context)}/{ProblemDetails.GetProblemNumber()}",
        };
}
