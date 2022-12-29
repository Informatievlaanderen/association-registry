namespace AssociationRegistry.Admin.Api.Infrastructure;

using System.Collections.Generic;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
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
                            new("NotEmptyValidator", "'Naam' mag niet leeg zijn."),
                        })
                },
            },
        };
}
