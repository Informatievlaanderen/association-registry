namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.RequestValidating.Contactgegevens.Waarde;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegevenValue_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var result = validator.TestValidate(
            new RegistreerFeitelijkeVerenigingRequest
            {
                Contactgegevens =
                    new[]
                    {
                        new ToeTeVoegenContactgegeven
                        {
                            Waarde = "",
                        },
                    },
            });

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Contactgegevens)}[0].{nameof(ToeTeVoegenContactgegeven.Waarde)}")
            .WithErrorMessage("'Waarde' mag niet leeg zijn.")
            .Only();
    }
}
