﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using FluentValidation.TestHelper;
using Test.Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class With_A_Valid_Locatietype : ValidatorTest
{
    [Theory]
    [InlineData(nameof(Locatietype.Correspondentie))]
    [InlineData(nameof(Locatietype.Activiteiten))]
    public void Has_no_validation_errors(string locationType)
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new RegistreerFeitelijkeVerenigingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = locationType,
                    Adres = new AssociationRegistry.Admin.Api.Verenigingen.Common.Adres
                    {
                        Straatnaam = "dezeStraat",
                        Huisnummer = "23",
                        Gemeente = "Zonnedorp",
                        Postcode = "0123",
                        Land = "Belgie",
                    },
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Adres.Gemeente)}");
    }
}
