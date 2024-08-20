<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Searching/RequestValidating/A_Valid_Request.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Searching.RequestValidating;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Searching.RequestValidating;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Searching/RequestValidating/A_Valid_Request.cs

using AssociationRegistry.Admin.Api.Verenigingen.Search.RequestModels;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
