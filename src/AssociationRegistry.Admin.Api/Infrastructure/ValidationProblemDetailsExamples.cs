namespace AssociationRegistry.Admin.Api.Infrastructure;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Hosts.Configuration.ConfigurationBindings;
using Swashbuckle.AspNetCore.Filters;

public class ValidationProblemDetailsExamples : IExamplesProvider<ValidationProblemDetails>
{
    public ValidationProblemDetails GetExamples()
        => new()
        {
            ValidationErrors = new Dictionary<string, ValidationProblemDetails.Errors>
            {
                {
                    "naam",
                    new ValidationProblemDetails.Errors(
                        new List<ValidationError>
                        {
                            new(code: "NotEmptyValidator", reason: "'Naam' mag niet leeg zijn."),
                        })
                },
            },
        };
}

public class BadRequestProblemDetailsExamples : IExamplesProvider<ProblemDetails>
{
    private readonly ProblemDetailsHelper _helper;
    private readonly AppSettings _appSettings;

    public BadRequestProblemDetailsExamples(ProblemDetailsHelper helper, AppSettings appSettings)
    {
        _helper = helper;
        _appSettings = appSettings;
    }

    public ProblemDetails GetExamples()
        => new()
        {
            HttpStatus = StatusCodes.Status400BadRequest,
            Title = ProblemDetails.DefaultTitle,
            Detail = "Beschrijving van het probleem",
            ProblemTypeUri = "urn:associationregistry.admin.api:validation",
            ProblemInstanceUri = $"{_appSettings.BaseUrl}/v1/foutmeldingen/{ProblemDetails.GetProblemNumber()}",
        };
}

public class ProblemAndValidationProblemDetailsExamples : IMultipleExamplesProvider<ProblemDetails>
{
    private readonly ProblemDetailsHelper _helper;
    private readonly AppSettings _appSettings;

    public ProblemAndValidationProblemDetailsExamples(ProblemDetailsHelper helper, AppSettings appSettings)
    {
        _helper = helper;
        _appSettings = appSettings;
    }

    public IEnumerable<SwaggerExample<ProblemDetails>> GetExamples()
        => new[]
        {
            SwaggerExample.Create(
                name: "Problem details zonder validatie fouten",
                new ProblemDetails
                {
                    HttpStatus = StatusCodes.Status400BadRequest,
                    Title = ProblemDetails.DefaultTitle,
                    Detail = "Beschrijving van het probleem",
                    ProblemTypeUri = "urn:associationregistry.admin.api:validation",
                    ProblemInstanceUri = $"{_appSettings.BaseUrl}/v1/foutmeldingen/{ProblemDetails.GetProblemNumber()}",
                }),
            new SwaggerExample<ProblemDetails>
            {
                Name = "Problem details met validatie fouten",
                Value =
                    new ValidationProblemDetails
                    {
                        ValidationErrors = new Dictionary<string, ValidationProblemDetails.Errors>
                        {
                            {
                                "naam",
                                new ValidationProblemDetails.Errors(
                                    new List<ValidationError>
                                    {
                                        new(code: "NotEmptyValidator", reason: "'Naam' mag niet leeg zijn."),
                                    })
                            },
                        },
                    },
            },
        };
}
