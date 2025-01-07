namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.
    Werkingsgebieden;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error_for_werkingsgebiedenLijst()
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var request = new WijzigBasisgegevensRequest
        {
            Werkingsgebieden = null,
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Werkingsgebieden);
    }
}
