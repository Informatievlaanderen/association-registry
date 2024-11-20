namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.
    Werkingsgebieden;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_NVT : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors_when_list_has_no_other_entries()
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var result = validator.TestValidate(new WijzigBasisgegevensRequest
        {
            Werkingsgebieden = ["NVT"],
        });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Werkingsgebieden);
    }

    [Fact]
    public void Has_validation_errors_when_list_has_more_entries()
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var result = validator.TestValidate(new WijzigBasisgegevensRequest
        {
            Werkingsgebieden = ["NVT", "BE25"],
        });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Werkingsgebieden)
              .WithErrorMessage("De waarde NVT in de werkingsgebiedenLijst mag niet met andere waarden gecombineerd worden.");

        ;
    }
}
