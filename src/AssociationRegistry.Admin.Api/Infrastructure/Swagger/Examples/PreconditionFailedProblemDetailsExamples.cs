namespace AssociationRegistry.Admin.Api.Infrastructure.Swagger.Examples;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;

public abstract class PreconditionFailedProblemDetailsExamplesBase : IExamplesProvider<ProblemDetails>
{
    private readonly ProblemDetailsHelper _helper;
    private readonly AppSettings _appSettings;
    private readonly string _message;

    protected PreconditionFailedProblemDetailsExamplesBase(ProblemDetailsHelper helper, AppSettings appSettings, string message)
    {
        _helper = helper;
        _appSettings = appSettings;

        _message = message;
    }

    public ProblemDetails GetExamples()
        => new()
        {
            HttpStatus = StatusCodes.Status412PreconditionFailed,
            Title = ProblemDetails.DefaultTitle,
            Detail = _message,
            ProblemTypeUri = "urn:associationregistry.admin.api:validation",
            ProblemInstanceUri = $"{_appSettings.BaseUrl}/v1/foutmeldingen/{ProblemDetails.GetProblemNumber()}",
        };
}

public class PreconditionFailedProblemDetailsExamples : PreconditionFailedProblemDetailsExamplesBase
{
    public PreconditionFailedProblemDetailsExamples(ProblemDetailsHelper helper, AppSettings appSettings) :
        base(helper, appSettings, ValidationMessages.Status412PreconditionFailed)
    {
    }
}

public class HistoriekPreconditionFailedProblemDetailsExamples : PreconditionFailedProblemDetailsExamplesBase
{
    public HistoriekPreconditionFailedProblemDetailsExamples(ProblemDetailsHelper helper, AppSettings appSettings) :
        base(helper, appSettings, ValidationMessages.Status412Historiek)
    {
    }
}

public class DetailPreconditionFailedProblemDetailsExamples : PreconditionFailedProblemDetailsExamplesBase
{
    public DetailPreconditionFailedProblemDetailsExamples(ProblemDetailsHelper helper, AppSettings appSettings) :
        base(helper, appSettings, ValidationMessages.Status412Detail)
    {
    }
}
