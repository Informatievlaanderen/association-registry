namespace AssociationRegistry.Test.Admin.Api.Commands.CommonValidators.DoelgroepRequestValidator.Maximumleeftijd;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_errors()
    {
        var validator = new DoelgroepRequestValidator();

        var request = new DoelgroepRequest
        {
            Maximumleeftijd = null,
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(r => r.Maximumleeftijd);
    }
}
