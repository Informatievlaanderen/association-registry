namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestValidating.A_Locaties;

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
public class With_Two_Different_Locations : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));

        var eersteLocatie = new ToeTeVoegenLocatie
        {
            Locatietype = Locatietype.Activiteiten,
            Adres = new Adres
            {
                Huisnummer = "23",
                Gemeente = "Zonnedorp",
                Postcode = "0123",
                Straatnaam = "Kerkstraat",
                Land = "België",
            },
        };

        var andereLocatie = new ToeTeVoegenLocatie
        {
            Locatietype = Locatietype.Activiteiten,
            Adres = new Adres
            {
                Huisnummer = "23",
                Gemeente = "Anderdorp",
                Postcode = "0123",
                Straatnaam = "Kerkstraat",
                Land = "België",
            },
        };

        var request = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
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
