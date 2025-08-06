namespace AssociationRegistry.Test.Public.Api.When_Searching.RequestValidating;

using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Api.WebApi.Verenigingen.Search.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class A_Valid_Request : ValidatorTest
{
    [Theory]
    [InlineData(100, 900)]
    [InlineData(0, 0)]
    [InlineData(0, 1000)]
    [InlineData(250, 250)]
    public void Has_no_validation_errors(int limit, int offset)
    {
        var appSettings = new AppSettings { Search = new AppSettings.SearchSettings { MaxNumberOfSearchResults = 1000 } };

        var validator = new PaginationQueryParamsValidator(appSettings);
        var result = validator.TestValidate(new PaginationQueryParams { Limit = limit, Offset = offset });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
