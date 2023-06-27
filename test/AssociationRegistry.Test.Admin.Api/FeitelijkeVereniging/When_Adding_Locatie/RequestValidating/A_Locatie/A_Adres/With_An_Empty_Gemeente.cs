namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Locatie.RequestValidating.A_Locatie.A_Adres;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using Framework;
using Vereniging;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Empty_Gemeente : ValidatorTest
{
    [Fact]
    public void Has_validation_error__gemeente_mag_niet_leeg_zijn()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new VoegLocatieToeRequest
        {
            Locatie = new ToeTeVoegenLocatie
            {
                Locatietype = Locatietype.Activiteiten,
                Adres = new ToeTeVoegenAdres
                {
                    Straatnaam = "Dezestraat",
                    Huisnummer = "23",
                    Postcode = "0123",
                    Land = "Belgie",
                    Gemeente = string.Empty,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                $"{nameof(VoegLocatieToeRequest.Locatie)}.{nameof(ToeTeVoegenLocatie.Adres)}.{nameof(ToeTeVoegenLocatie.Adres.Gemeente)}")
            .WithErrorMessage("'Gemeente' mag niet leeg zijn.");
    }
}
