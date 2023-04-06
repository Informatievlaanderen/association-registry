namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Contactgegeven.RequestValidating.ContactGegeven.Waarde;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.WijzigContactgegeven;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegevenValue_mag_niet_leeg_zijn()
    {
        var validator = new WijzigContactgegevenValidator();
        var result = validator.TestValidate(
            new WijzigContactgegevenRequest
            {
                Contactgegeven = new WijzigContactgegevenRequest.RequestContactgegeven
                    {
                        Waarde = "",
                    },
            });

        result.ShouldHaveValidationErrorFor(nameof(WijzigContactgegevenRequest.Contactgegeven)+"."+nameof(WijzigContactgegevenRequest.RequestContactgegeven.Waarde))
            .WithErrorMessage("'Waarde' mag niet leeg zijn.")
            .Only();
    }
}
