﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Adding_Locatie.RequestValidating.A_Locatie.A_Adres;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;

[UnitTest]
public class With_An_Empty_Postcode : ValidatorTest
{
    [Fact]
    public void Has_validation_error__postcode_mag_niet_leeg_zijn()
    {
        var validator = new VoegLocatieToeValidator();

        var request = new VoegLocatieToeRequest
        {
            Locatie =
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietype.Activiteiten,
                    Adres = new Adres
                    {
                        Straatnaam = "Dezestraat",
                        Gemeente = "Zonnedorp",
                        Huisnummer = "23",
                        Land = "Belgie",
                        Postcode = string.Empty,
                    },
                },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(VoegLocatieToeRequest.Locatie)}.{nameof(ToeTeVoegenLocatie.Adres)}.{nameof(ToeTeVoegenLocatie.Adres.Postcode)}")
              .WithErrorMessage("'Postcode' mag niet leeg zijn.");
    }
}
