namespace AssociationRegistry.Test.Admin.Api.CommonValidators.DoelgroepRequestValidator.Minimumleeftijd;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Outside_Of_Range : ValidatorTest
{
    [Theory]
    [InlineData(-1)]
    [InlineData(160)]
    [InlineData(151)]
    public void Has_validation_errors(int minleeftijd)
    {
        var validator = new DoelgroepRequestValidator();

        var request = new DoelgroepRequest
        {
            Minimumleeftijd = minleeftijd,
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Minimumleeftijd)
              .WithErrorMessage("De 'minimumleeftijd' moet binnen 0 en 150 vallen.");
    }
}
