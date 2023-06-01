﻿namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Empty_Land : ValidatorTest
{
    [Fact]
    public void Has_validation_error__land_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerAfdelingRequestValidator();
        var request = new RegistreerAfdelingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietypes.Activiteiten,
                    Straatnaam = "Dezestraat",
                    Huisnummer = "23",
                    Postcode = "0123",
                    Gemeente = "Hottentot",
                    Land = string.Empty,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerAfdelingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Land)}")
            .WithErrorMessage("'Land' mag niet leeg zijn.");
    }
}
