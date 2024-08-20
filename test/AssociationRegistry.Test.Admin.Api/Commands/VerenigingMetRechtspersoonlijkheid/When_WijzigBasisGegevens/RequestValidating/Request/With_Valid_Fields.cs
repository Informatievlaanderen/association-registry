namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.Request;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Valid_Fields
{
    [Theory]
    [InlineData("beschrijving", null, null, null, null)]
    [InlineData(null, new[] { "abcd" }, null, null, null)]
    [InlineData(null, null, "roepnaam", null, null)]
    [InlineData(null, null, "roepnaam", 0, null)]
    [InlineData(null, null, "roepnaam", null, 1)]
    [InlineData(null, null, "roepnaam", 0, 1)]
    [InlineData("beschrijving", new[] { "abcd" }, "roepnaam", 1, 2)]
    public void Then_it_should_not_have_errors(
        string? korteBeschrijving,
        string[]? hoofdactiviteiten,
        string? roepnaam,
        int? minimumLeeftijd,
        int? maximumLeeftijd)
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var result = validator.TestValidate(
            new WijzigBasisgegevensRequest
            {
                KorteBeschrijving = korteBeschrijving,
                HoofdactiviteitenVerenigingsloket = hoofdactiviteiten,
                Roepnaam = roepnaam,
                Doelgroep = new DoelgroepRequest
                {
                    Minimumleeftijd = minimumLeeftijd,
                    Maximumleeftijd = maximumLeeftijd,
                },
            });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
