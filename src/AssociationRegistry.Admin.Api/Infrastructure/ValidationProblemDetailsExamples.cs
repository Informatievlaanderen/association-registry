namespace AssociationRegistry.Admin.Api.Infrastructure;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

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
    private readonly IHttpContextAccessor _contextAccessor;

    public BadRequestProblemDetailsExamples(ProblemDetailsHelper helper, IHttpContextAccessor contextAccessor)
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
            ProblemTypeUri = "urn:associationregistry.admin.api:validation",
            ProblemInstanceUri = $"{_helper.GetInstanceBaseUri(_contextAccessor.HttpContext)}/{ProblemDetails.GetProblemNumber()}",
        };
}

public class ProblemAndValidationProblemDetailsExamples : IMultipleExamplesProvider<ProblemDetails>
{
    private readonly ProblemDetailsHelper _helper;
    private readonly IHttpContextAccessor _contextAccessor;

    public ProblemAndValidationProblemDetailsExamples(ProblemDetailsHelper helper, IHttpContextAccessor contextAccessor)
    {
        _helper = helper;
        _contextAccessor = contextAccessor;
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
                    ProblemInstanceUri = $"{_helper.GetInstanceBaseUri(_contextAccessor.HttpContext)}/{ProblemDetails.GetProblemNumber()}",
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
