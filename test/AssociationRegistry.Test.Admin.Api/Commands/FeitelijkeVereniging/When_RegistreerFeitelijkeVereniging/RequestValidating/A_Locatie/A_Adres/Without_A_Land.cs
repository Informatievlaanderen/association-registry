namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locatie.
    A_Adres;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using FluentValidation.TestHelper;
using Test.Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common.Adres;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class Without_A_Land : ValidatorTest
{
    [Fact]
    public void Has_validation_error__land_is_verplicht()
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
                        Huisnummer = "23",
                        Postcode = "0123",
                        Gemeente = "Hottentot",
                    },
                },
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.Adres)}.{nameof(ToeTeVoegenLocatie.Adres.Land)}")
              .WithErrorMessage("'Land' is verplicht.");
    }
}
