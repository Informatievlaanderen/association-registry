namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingZonderEigenRechtspersoonlijkheid.When_Registreer.RequestValidating.
    A_WerkingsgebiedenLijst;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequetsModels;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Framework;
using Vereniging;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class Is_NVT : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors_when_list_has_no_other_entries()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));

        var result = validator.TestValidate(new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
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
