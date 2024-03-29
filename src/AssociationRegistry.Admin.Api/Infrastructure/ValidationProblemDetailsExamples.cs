﻿namespace AssociationRegistry.Admin.Api.Infrastructure;

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

    public BadRequestProblemDetailsExamples(ProblemDetailsHelper helper)
    {
        _helper = helper;
    }

    public ProblemDetails GetExamples()
        => new()
        {
            HttpStatus = StatusCodes.Status400BadRequest,
            Title = ProblemDetails.DefaultTitle,
            Detail = "Beschrijving van het probleem",
            ProblemTypeUri = "urn:associationregistry.admin.api:validation",
            ProblemInstanceUri = $"{_helper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}",
        };
}

public class ProblemAndValidationProblemDetailsExamples : IMultipleExamplesProvider<ProblemDetails>
{
    private readonly ProblemDetailsHelper _helper;

    public ProblemAndValidationProblemDetailsExamples(ProblemDetailsHelper helper)
    {
        _helper = helper;
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
                    ProblemInstanceUri = $"{_helper.GetInstanceBaseUri()}/{ProblemDetails.GetProblemNumber()}",
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
