namespace AssociationRegistry.Admin.Api.Infrastructure;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Microsoft.AspNetCore.Http;

public class BadHttpRequestExceptionHandler : DefaultExceptionHandler<BadHttpRequestException>
{
    private readonly ProblemDetailsHelper _problemDetailsHelper;

    public BadHttpRequestExceptionHandler(ProblemDetailsHelper problemDetailsHelper)
    {
        _problemDetailsHelper = problemDetailsHelper;
    }

    protected override ProblemDetails GetApiProblemFor(BadHttpRequestException exception)
        => new()
        {
            HttpStatus = StatusCodes.Status400BadRequest,
            Title = ProblemDetails.DefaultTitle,
            Detail = exception.Message,
            ProblemTypeUri = _problemDetailsHelper.GetExceptionTypeUriFor(exception),
            ProblemInstanceUri = $"{_problemDetailsHelper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}",
        };
}
