namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Locatie.An_Adres;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling.RequestModels;
using FluentValidation.TestHelper;
using Test.Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class With_An_Empty_Land : ValidatorTest
{
    [Fact]
    public void Has_validation_error__land_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerAfdelingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new RegistreerAfdelingRequest
        {
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietype.Activiteiten,
                    Adres = new AssociationRegistry.Admin.Api.Verenigingen.Common.Adres
                    {
                        Straatnaam = "Dezestraat",
                        Huisnummer = "23",
                        Postcode = "0123",
                        Gemeente = "Hottentot",
                        Land = string.Empty,
                    },
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerAfdelingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Adres)}.{nameof(ToeTeVoegenLocatie.Adres.Land)}")
            .WithErrorMessage("'Land' mag niet leeg zijn.");
    }
}
