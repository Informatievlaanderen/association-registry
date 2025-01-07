namespace AssociationRegistry.Test.Admin.Api.Commands.CommonValidators.DoelgroepRequestValidator;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Invalid : ValidatorTest
{
    [Fact]
    public void Has_validation_errors()
    {
        var validator = new DoelgroepRequestValidator();

        var request = new DoelgroepRequest
        {
            Minimumleeftijd = 100,
            Maximumleeftijd = 50,
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r)
              .WithErrorMessage("Bij 'doelgroep' moet de 'minimumleeftijd' kleiner of gelijk aan 'maximumleeftijd' zijn.");
    }
}
