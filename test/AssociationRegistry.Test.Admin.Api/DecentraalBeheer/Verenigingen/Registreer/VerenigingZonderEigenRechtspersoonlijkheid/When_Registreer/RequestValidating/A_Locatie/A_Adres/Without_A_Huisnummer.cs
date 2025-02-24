﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.When_Registreer.RequestValidating.A_Locatie.
    A_Adres;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class Without_A_Huisnummer : ValidatorTest
{
    [Fact]
    public void Has_validation_error__huisnummer_is_verplicht()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));

        var request = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietype.Activiteiten,
                    Adres = new Adres
                    {
                        Straatnaam = "Dezestraat",
                        Gemeente = "Zonnedorp",
                        Postcode = "0123",
                        Land = "België",
                    },
                },
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Adres)}.{nameof(ToeTeVoegenLocatie.Adres.Huisnummer)}")
              .WithErrorMessage("'Huisnummer' is verplicht.");
    }
}
