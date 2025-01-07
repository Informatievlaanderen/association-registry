namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.
    Werkingsgebieden;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var result = validator.TestValidate(new WijzigBasisgegevensRequest
        {
            Werkingsgebieden = new[] { "BE25" },
        });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Werkingsgebieden);
    }
}
