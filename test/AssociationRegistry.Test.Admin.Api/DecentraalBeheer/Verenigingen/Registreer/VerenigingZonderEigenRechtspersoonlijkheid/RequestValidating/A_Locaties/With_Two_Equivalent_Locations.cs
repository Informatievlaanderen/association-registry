namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestValidating.A_Locaties;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using FluentValidation.TestHelper;
using Xunit;
using Adres = AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common.Adres;
using ValidatorTest = Framework.ValidatorTest;

public class With_Two_Equivalent_Locations : ValidatorTest
{
    [Fact]
    public void Has_validation_error__equivalent_locaties_verboden()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(
            clock: new ClockStub(now: DateOnly.MaxValue)
        );

        var equivalentLocatie = new ToeTeVoegenLocatie
        {
            Locatietype = Locatietype.Activiteiten,
            Adres = new Adres
            {
                Huisnummer = "23",
                Gemeente = "Zonnedorp",
                Postcode = "0123",
                Land = "België",
            },
        };

        var equivalentLocatie2 = new ToeTeVoegenLocatie
        {
            Locatietype = Locatietype.Activiteiten,
            Adres = new Adres
            {
                Huisnummer = "2*/3",
                Gemeente = "Zonnedor p",
                Postcode = "012-3",
                Land = "Belgie",
            },
        };

        var request = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
        {
            Locaties = [equivalentLocatie, equivalentLocatie2],
        };

        var result = validator.TestValidate(objectToTest: request);

        result
            .ShouldHaveValidationErrorFor(memberAccessor: vereniging => vereniging.Locaties)
            .WithErrorMessage(expectedErrorMessage: "Identieke locaties zijn niet toegelaten.");
    }
}
