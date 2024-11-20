namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.
    Werkingsgebieden;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Vereniging;
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
            Werkingsgebieden = [ "NVT" ],
        });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Werkingsgebieden);
    }

    [Fact]
    public void Has_validation_errors_when_list_has_more_entries()
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var result = validator.TestValidate(new WijzigBasisgegevensRequest
        {
            Werkingsgebieden = [ "NVT", "BE25" ],
        });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Werkingsgebieden)
              .WithErrorMessage("Elke waarde in de werkingsgebieden mag slechts 1 maal voorkomen.");;
    }

}
