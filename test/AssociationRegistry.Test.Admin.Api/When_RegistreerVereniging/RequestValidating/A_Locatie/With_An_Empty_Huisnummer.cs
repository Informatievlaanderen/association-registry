﻿namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Empty_Huisnummer : ValidatorTest
{
    [Fact]
    public void Has_validation_error__huisnummer_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietypes.Activiteiten,
                    Straatnaam = "Dezestraat",
                    Huisnummer = string.Empty,
                    Gemeente = "Zonnedorp",
                    Postcode = "0123",
                    Land = "Belgie",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Huisnummer)}")
            .WithErrorMessage("'Huisnummer' mag niet leeg zijn.");
    }
}
