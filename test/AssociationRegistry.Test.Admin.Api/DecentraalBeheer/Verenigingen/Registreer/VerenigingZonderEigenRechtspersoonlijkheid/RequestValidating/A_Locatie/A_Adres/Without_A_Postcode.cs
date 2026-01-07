namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestValidating.A_Locatie.
    A_Adres;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using FluentValidation.TestHelper;
using Xunit;
using Adres = AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common.Adres;
using ValidatorTest = Framework.ValidatorTest;

public class Without_A_Postcode : ValidatorTest
{
    [Fact]
    public void Has_validation_error__postcode_is_verplicht()
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
                        Huisnummer = "23",
                        Land = "België",
                    },
                },
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Adres)}.{nameof(ToeTeVoegenLocatie.Adres.Postcode)}")
              .WithErrorMessage("'postcode' is verplicht.");
    }
}
