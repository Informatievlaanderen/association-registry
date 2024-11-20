namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.
    A_WerkingsgebiedenLijst;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using FluentValidation.TestHelper;
using Test.Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class Is_NVT : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors_when_list_has_no_other_entries()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));

        var result = validator.TestValidate(new RegistreerFeitelijkeVerenigingRequest
        {
            Werkingsgebieden = [Werkingsgebied.NietVanToepassing.Code],
        });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Werkingsgebieden);
    }

    [Fact]
    public void Has_validation_errors_when_list_has_more_entries()
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var result = validator.TestValidate(new WijzigBasisgegevensRequest
        {
            Werkingsgebieden =
            [
                Werkingsgebied.NietVanToepassing.Code,
                "BE25",
            ],
        });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Werkingsgebieden)
              .WithErrorMessage(ValidationMessages.WerkingsgebiedKanNietGecombineerdWordenMetNVT);
    }
}
