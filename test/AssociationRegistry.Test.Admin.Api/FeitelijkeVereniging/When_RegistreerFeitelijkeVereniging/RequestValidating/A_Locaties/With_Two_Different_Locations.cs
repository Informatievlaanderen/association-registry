﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locaties;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using Framework;
using FluentValidation.TestHelper;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Different_Locations : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var eersteLocatie = new ToeTeVoegenLocatie
        {
            Locatietype = Locatietype.Activiteiten,
            Adres = new ToeTeVoegenAdres
            {
                Huisnummer = "23",
                Gemeente = "Zonnedorp",
                Postcode = "0123",
                Straatnaam = "Kerkstraat",
                Land = "Belgie",
            },
        };
        var andereLocatie = new ToeTeVoegenLocatie
        {
            Locatietype = Locatietype.Activiteiten,
            Adres = new ToeTeVoegenAdres
            {
                Huisnummer = "23",
                Gemeente = "Anderdorp",
                Postcode = "0123",
                Straatnaam = "Kerkstraat",
                Land = "Belgie",
            },
        };
        var request = new RegistreerFeitelijkeVerenigingRequest
        {
            Locaties = new[]
            {
                eersteLocatie,
                andereLocatie,
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(r => r.Locaties);
    }
}
