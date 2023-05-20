namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.RequestValidating.Contactgegevens.Type;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class Is_Null : ValidatorTest
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
                            Type = null!,
                        },
                    },
            });

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Contactgegevens)}[0].{nameof(ToeTeVoegenContactgegeven.Type)}")
            .WithErrorMessage("'Type' is verplicht.")
            .Only();
    }
}
