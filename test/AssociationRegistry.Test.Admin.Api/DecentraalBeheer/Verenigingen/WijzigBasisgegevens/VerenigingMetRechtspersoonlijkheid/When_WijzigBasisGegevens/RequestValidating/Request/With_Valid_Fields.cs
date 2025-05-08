namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.Request;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using FluentValidation.TestHelper;
using Xunit;

public class With_Valid_Fields
{
    [Theory]
    [InlineData("beschrijving", null, null, null, null, null)]
    [InlineData(null, new[] { "abcd" }, null, null, null, null)]
    [InlineData(null, null, null, "roepnaam", null, null)]
    [InlineData(null, null, null, "roepnaam", 0, null)]
    [InlineData(null, null, null, "roepnaam", null, 1)]
    [InlineData(null, null, null, "roepnaam", 0, 1)]
    [InlineData("beschrijving", new[] { "abcd" }, null, "roepnaam", 1, 2)]
    [InlineData(null, null, new[] { "BE25" }, null, null, null)]
    public void Then_it_should_not_have_errors(
        string? korteBeschrijving,
        string[]? hoofdactiviteiten,
        string[]? werkingsgebieden,
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
                Werkingsgebieden = werkingsgebieden,
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
