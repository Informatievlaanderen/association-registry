namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.
    HoofdActiviteitenLijst;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Framework;
using FluentValidation.TestHelper;
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
            HoofdactiviteitenVerenigingsloket = new[] { "abcd" },
        });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.KorteBeschrijving);
    }
}
