namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Adding_Contactgegeven.RequestValidating.ContactGegeven.Type;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Null : ValidatorTest
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
                    Contactgegeventype = null!,
                },
            });

        result.ShouldHaveValidationErrorFor(nameof(VoegContactgegevenToeRequest.Contactgegeven) + "." +
                                            nameof(ToeTeVoegenContactgegeven.Contactgegeventype))
              .WithErrorMessage("'Contactgegeventype' is verplicht.");
        // .Only();
    }
}
