namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.Request;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_Fields
{
    [Theory]
    [InlineData("beschrijving", null)]
    [InlineData(null, new[] {"abcd"})]
    [InlineData("beschrijving", new[] {"abcd"})]
    public void Then_it_should_not_have_errors(string? korteBeschrijving, string[] hoofdactiviteiten)
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var result = validator.TestValidate(
            new WijzigBasisgegevensRequest
            {
                KorteBeschrijving = korteBeschrijving,
                HoofdactiviteitenVerenigingsloket = hoofdactiviteiten,
            });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
