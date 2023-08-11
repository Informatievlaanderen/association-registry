namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Locatie.RequestValidating.A_Locatie.A_Adres;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using Framework;
using Vereniging;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Empty_Straatnaam : ValidatorTest
{
    [Fact]
    public void Has_validation_error__straatnaam_mag_niet_leeg_zijn()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new VoegLocatieToeRequest
        {
            Locatie =
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietype.Activiteiten,
                    Adres = new AssociationRegistry.Admin.Api.Verenigingen.Common.Adres
                    {
                        Straatnaam = string.Empty,
                        Huisnummer = "23",
                        Gemeente = "Zonnedorp",
                        Postcode = "0123",
                        Land = "Belgie",
                    },
                },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(VoegLocatieToeRequest.Locatie)}.{nameof(ToeTeVoegenLocatie.Adres)}.{nameof(ToeTeVoegenLocatie.Adres.Straatnaam)}")
            .WithErrorMessage("'Straatnaam' mag niet leeg zijn.");
    }
}
