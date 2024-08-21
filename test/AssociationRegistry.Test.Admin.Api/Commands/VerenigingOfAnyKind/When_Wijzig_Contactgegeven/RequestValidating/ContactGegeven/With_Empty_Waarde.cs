namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Contactgegeven.RequestValidating.ContactGegeven;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven.RequestModels;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class With_Empty_Waarde : ValidatorTest
{
    [Fact] public void Has_validation_error__waarde_mag_niet_leeg_zijn()
    {
        var validator = new WijzigContactgegevenValidator();

        var request = new WijzigContactgegevenRequest
        {
            Contactgegeven = new TeWijzigenContactgegeven
            {
                Waarde = string.Empty,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Contactgegeven.Waarde)
              .WithErrorMessage("'Waarde' mag niet leeg zijn.");
    }
}
