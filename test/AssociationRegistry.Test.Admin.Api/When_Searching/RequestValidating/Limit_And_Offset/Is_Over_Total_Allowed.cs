namespace AssociationRegistry.Test.Admin.Api.When_Searching.RequestValidating.A_Naam;

using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using AssociationRegistry.Admin.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
                    MaxNumberOfSearchResults = 1000
                }
            });

        var result = validator.TestValidate(new PaginationQueryParams { Limit = 101, Offset = 900 });

        result.ShouldHaveValidationErrorFor(queryParams => queryParams)
            .WithErrorMessage("'Limit' en 'Offset' mogen samen niet groter dan 1000 zijn.")
            .Only();
    }
}
