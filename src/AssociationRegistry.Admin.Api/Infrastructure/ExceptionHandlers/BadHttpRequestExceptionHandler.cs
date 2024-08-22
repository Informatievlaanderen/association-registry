namespace AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;

public class BadHttpRequestExceptionHandler : DefaultExceptionHandler<BadHttpRequestException>
{
    private readonly ProblemDetailsHelper _problemDetailsHelper;

    public BadHttpRequestExceptionHandler(ProblemDetailsHelper problemDetailsHelper)
    {
        _problemDetailsHelper = problemDetailsHelper;
    }

    protected override ProblemDetails GetApiProblemFor(HttpContext context, BadHttpRequestException exception)
        => new()
        {
            HttpStatus = StatusCodes.Status400BadRequest,
            Title = ProblemDetails.DefaultTitle,
            Detail = exception.Message,
            ProblemTypeUri = _problemDetailsHelper.GetExceptionTypeUriFor(exception),
            ProblemInstanceUri = $"{_problemDetailsHelper.GetInstanceBaseUri(context)}/{ProblemDetails.GetProblemNumber()}",
        };
}
