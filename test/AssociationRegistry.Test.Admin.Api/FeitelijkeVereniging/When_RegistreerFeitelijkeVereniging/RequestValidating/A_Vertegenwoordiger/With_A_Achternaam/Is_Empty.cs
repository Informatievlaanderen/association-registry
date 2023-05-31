namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Vertegenwoordiger.With_A_Achternaam;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Empty
{
    [Fact]
    public void Has_validation_error__Achternaam_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerDecentraalBeheerdeVerenigingRequestValidator();
        var request = new RegistreerDecentraalBeheerdeVerenigingRequest
        {
            Vertegenwoordigers = new[]
            {
                new ToeTeVoegenVertegenwoordiger
                {
                    Achternaam = string.Empty,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                $"{nameof(RegistreerDecentraalBeheerdeVerenigingRequest.Vertegenwoordigers)}[0].{nameof(ToeTeVoegenVertegenwoordiger.Achternaam)}")
            .WithErrorMessage("'Achternaam' mag niet leeg zijn.");
    }
}
