namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.Request;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_Fields
{
    [Theory]
    [InlineData("beschrijving", null, null)]
    [InlineData(null, new[] { "abcd" }, null)]
    [InlineData(null, null, "roepnaam")]
    [InlineData("beschrijving", new[] { "abcd" }, "roepnaam")]
    public void Then_it_should_not_have_errors(string? korteBeschrijving, string[] hoofdactiviteiten, string roepnaam)
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var result = validator.TestValidate(
            new WijzigBasisgegevensRequest
            {
                KorteBeschrijving = korteBeschrijving,
                HoofdactiviteitenVerenigingsloket = hoofdactiviteiten,
                Roepnaam = roepnaam,
            });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
