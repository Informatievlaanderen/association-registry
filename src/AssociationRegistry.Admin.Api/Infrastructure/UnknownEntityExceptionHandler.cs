namespace AssociationRegistry.Admin.Api.Infrastructure;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using ContactGegevens.Exceptions;
using Microsoft.AspNetCore.Http;

public class UnknownEntityExceptionHandler : DefaultExceptionHandler<OnbekendContactgegeven>
{
    private readonly ProblemDetailsHelper _problemDetailsHelper;

    public UnknownEntityExceptionHandler(ProblemDetailsHelper problemDetailsHelper)
    {
        _problemDetailsHelper = problemDetailsHelper;
    }

    protected override ProblemDetails GetApiProblemFor(OnbekendContactgegeven exception)
        => new()
        {
            HttpStatus = StatusCodes.Status404NotFound,
            Title = ProblemDetails.DefaultTitle,
            Detail = exception.Message,
            ProblemTypeUri = _problemDetailsHelper.GetExceptionTypeUriFor(exception),
            ProblemInstanceUri = $"{_problemDetailsHelper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}",
        };
}
