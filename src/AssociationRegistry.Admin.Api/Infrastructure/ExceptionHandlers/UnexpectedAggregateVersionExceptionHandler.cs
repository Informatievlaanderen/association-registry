namespace AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using EventStore;

public class UnexpectedAggregateVersionExceptionHandler : DefaultExceptionHandler<UnexpectedAggregateVersionException>
{
    private readonly ProblemDetailsHelper _problemDetailsHelper;

    public UnexpectedAggregateVersionExceptionHandler(ProblemDetailsHelper problemDetailsHelper)
    {
        _problemDetailsHelper = problemDetailsHelper;
    }

    protected override ProblemDetails GetApiProblemFor(HttpContext context, UnexpectedAggregateVersionException exception)
        => new()
        {
            HttpStatus = StatusCodes.Status412PreconditionFailed,
            Title = ProblemDetails.DefaultTitle,
            Detail = exception.Message,
            ProblemTypeUri = _problemDetailsHelper.GetExceptionTypeUriFor(exception),
            ProblemInstanceUri = $"{_problemDetailsHelper.GetInstanceBaseUri(context)}/{ProblemDetails.GetProblemNumber()}",
        };
}
