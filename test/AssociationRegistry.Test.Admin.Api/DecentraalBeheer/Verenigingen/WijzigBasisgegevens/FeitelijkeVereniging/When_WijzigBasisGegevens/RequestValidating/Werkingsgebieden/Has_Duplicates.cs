namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.
    Werkingsgebieden;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Has_Duplicates : ValidatorTest
{
    [Theory]
    [InlineData("ABCD", "ABCD")]
    [InlineData("Test", "tEST")]
    [InlineData("BLABLAbla", "BlAbLaBlA")]
    public void Has_a_validation_error_for_werkingsgebieden(string code1, string code2)
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var request = new WijzigBasisgegevensRequest
        {
            Werkingsgebieden = new[] { code1, code2 },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Werkingsgebieden)
              .WithErrorMessage(ValidationMessages.WerkingsgebiedMagSlechtsEenmaalVoorkomen);
    }
}
