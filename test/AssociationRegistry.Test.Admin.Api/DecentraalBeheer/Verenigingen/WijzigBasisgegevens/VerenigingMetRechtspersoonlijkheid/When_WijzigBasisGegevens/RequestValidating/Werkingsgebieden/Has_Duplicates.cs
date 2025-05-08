namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.
    Werkingsgebieden;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Has_Duplicates : ValidatorTest
{
    [Theory]
    [InlineData("BE21", "BE21")]
    [InlineData("BE25", "BE25")]
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
