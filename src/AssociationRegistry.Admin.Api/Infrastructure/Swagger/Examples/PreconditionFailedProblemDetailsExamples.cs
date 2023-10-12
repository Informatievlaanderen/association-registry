namespace AssociationRegistry.Admin.Api.Infrastructure.Swagger.Examples;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;

public abstract class PreconditionFailedProblemDetailsExamplesBase : IExamplesProvider<ProblemDetails>
{
    private readonly ProblemDetailsHelper _helper;
    private readonly string _message;

    protected PreconditionFailedProblemDetailsExamplesBase(ProblemDetailsHelper helper, string message)
    {
        _helper = helper;
        _message = message;
    }

    public ProblemDetails GetExamples()
        => new()
        {
            HttpStatus = StatusCodes.Status412PreconditionFailed,
            Title = ProblemDetails.DefaultTitle,
            Detail = _message,
            ProblemTypeUri = "urn:associationregistry.admin.api:validation",
            ProblemInstanceUri = $"{_helper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}",
        };
}

public class PreconditionFailedProblemDetailsExamples : PreconditionFailedProblemDetailsExamplesBase
{
    public PreconditionFailedProblemDetailsExamples(ProblemDetailsHelper helper) :
        base(helper, ValidationMessages.Status412PreconditionFailed)
    {
    }
}

public class HistoriekPreconditionFailedProblemDetailsExamples : PreconditionFailedProblemDetailsExamplesBase
{
    public HistoriekPreconditionFailedProblemDetailsExamples(ProblemDetailsHelper helper) :
        base(helper, ValidationMessages.Status412Historiek)
    {
    }
}

public class DetailPreconditionFailedProblemDetailsExamples : PreconditionFailedProblemDetailsExamplesBase
{
    public DetailPreconditionFailedProblemDetailsExamples(ProblemDetailsHelper helper) :
        base(helper, ValidationMessages.Status412Detail)
    {
    }
}
