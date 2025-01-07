namespace AssociationRegistry.Test.Admin.Api.Commands.CommonValidators.DoelgroepRequestValidator;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid : ValidatorTest
{
    [Theory]
    [InlineData(0, 150)]
    [InlineData(150, 150)]
    [InlineData(50, 100)]
    [InlineData(20, null)]
    [InlineData(null, 14)]
    public void Has_no_validation_errors(int? minleeftijd, int? maxleeftijd)
    {
        var validator = new DoelgroepRequestValidator();

        var request = new DoelgroepRequest
        {
            Minimumleeftijd = minleeftijd,
            Maximumleeftijd = maxleeftijd,
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
