namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Contactgegeven.RequestValidating.ContactGegeven.
    Waarde;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegevenValue_mag_niet_leeg_zijn()
    {
        var validator = new VoegContactgegevenToeValidator();

        var result = validator.TestValidate(
            new VoegContactgegevenToeRequest
            {
                Contactgegeven = new ToeTeVoegenContactgegeven
                {
                    Waarde = "",
                },
            });

        result.ShouldHaveValidationErrorFor(nameof(VoegContactgegevenToeRequest.Contactgegeven) + "." +
                                            nameof(ToeTeVoegenContactgegeven.Waarde))
              .WithErrorMessage("'Waarde' mag niet leeg zijn.");
        // .Only();
    }
}
