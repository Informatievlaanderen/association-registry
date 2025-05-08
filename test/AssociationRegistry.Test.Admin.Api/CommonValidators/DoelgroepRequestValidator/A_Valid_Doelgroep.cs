namespace AssociationRegistry.Test.Admin.Api.CommonValidators.DoelgroepRequestValidator;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

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
