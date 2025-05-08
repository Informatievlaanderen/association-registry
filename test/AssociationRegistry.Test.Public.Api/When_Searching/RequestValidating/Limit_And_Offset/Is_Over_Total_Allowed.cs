namespace AssociationRegistry.Test.Public.Api.When_Searching.RequestValidating.Limit_And_Offset;

using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Api.Verenigingen.Search.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class Is_Over_Total_Allowed : ValidatorTest
{
    [Fact]
    public void Has_validation_error__for_query_params()
    {
        var validator = new PaginationQueryParamsValidator(
            new AppSettings
            {
                Search = new AppSettings.SearchSettings
                {
                    MaxNumberOfSearchResults = 1000,
                },
            });

        var result = validator.TestValidate(new PaginationQueryParams { Limit = 101, Offset = 900 });

        result.ShouldHaveValidationErrorFor(queryParams => queryParams)
              .WithErrorMessage("'Limit' en 'Offset' mogen samen niet groter dan 1000 zijn.");
        // .Only();
    }
}
