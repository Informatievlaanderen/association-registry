namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Searching.RequestValidating.Limit_And_Offset;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.RequestModels;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
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
