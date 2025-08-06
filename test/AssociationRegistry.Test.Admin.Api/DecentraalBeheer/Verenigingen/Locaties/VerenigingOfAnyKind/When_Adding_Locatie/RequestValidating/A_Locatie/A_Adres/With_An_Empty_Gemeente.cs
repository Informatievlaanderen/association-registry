namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Adding_Locatie.RequestValidating.A_Locatie.A_Adres;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using FluentValidation.TestHelper;
using Xunit;
using Adres = AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common.Adres;

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
                Adres = new Adres
                {
                    Straatnaam = "Dezestraat",
                    Huisnummer = "23",
                    Postcode = "0123",
                    Land = "België",
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
