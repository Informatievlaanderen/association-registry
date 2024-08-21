namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locaties;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Framework.Helpers;
using AssociationRegistry.Test.Framework;
using Vereniging;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class With_Two_Identical_Locations : ValidatorTest
{
    [Fact]
    public void Has_validation_error__identiek_locaties_verboden()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));

        var identiekLocatie = new ToeTeVoegenLocatie
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

        var request = new RegistreerFeitelijkeVerenigingRequest
        {
            Locaties = new[]
            {
                identiekLocatie.Copy(),
                identiekLocatie.Copy(),
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Locaties)
              .WithErrorMessage("Identieke locaties zijn niet toegelaten.");
    }
}
