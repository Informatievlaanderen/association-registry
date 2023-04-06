namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Contactgegeven.RequestValidating.ContactGegeven.Waarde;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.WijzigContactgegeven;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Null : ValidatorTest
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
                        Waarde = null!,
                    },
            });

        result.ShouldHaveValidationErrorFor(nameof(WijzigContactgegevenRequest.Contactgegeven) + "." + nameof(WijzigContactgegevenRequest.RequestContactgegeven.Waarde))
            .WithErrorMessage("'Waarde' is verplicht.")
            .Only();
    }
}
