namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.Contactgegevens.
    Type;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegevenValue_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));

        var result = validator.TestValidate(
            new RegistreerFeitelijkeVerenigingRequest
            {
                Contactgegevens =
                    new[]
                    {
                        new ToeTeVoegenContactgegeven
                        {
                            Contactgegeventype = null!,
                        },
                    },
            });

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(RegistreerFeitelijkeVerenigingRequest.Contactgegevens)}[0].{nameof(ToeTeVoegenContactgegeven.Contactgegeventype)}")
              .WithErrorMessage("'Contactgegeventype' is verplicht.");
        // .Only();
    }
}
