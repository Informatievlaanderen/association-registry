namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locaties;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using FluentValidation.TestHelper;
using Test.Framework;
using Vereniging;
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
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));

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
