﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequestValidating.A_Locatie.
    A_Adres;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using FluentValidation.TestHelper;
using Xunit;
using Adres = AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common.Adres;
using ValidatorTest = Framework.ValidatorTest;

public class With_An_Empty_Huisnummer : ValidatorTest
{
    [Fact]
    public void Has_validation_error__huisnummer_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));

        var request = new RegistreerFeitelijkeVerenigingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietype.Activiteiten,
                    Adres = new Adres
                    {
                        Straatnaam = "Dezestraat",
                        Huisnummer = string.Empty,
                        Gemeente = "Zonnedorp",
                        Postcode = "0123",
                        Land = "België",
                    },
                },
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Adres)}.{nameof(ToeTeVoegenLocatie.Adres.Huisnummer)}")
              .WithErrorMessage("'Huisnummer' mag niet leeg zijn.");
    }
}
