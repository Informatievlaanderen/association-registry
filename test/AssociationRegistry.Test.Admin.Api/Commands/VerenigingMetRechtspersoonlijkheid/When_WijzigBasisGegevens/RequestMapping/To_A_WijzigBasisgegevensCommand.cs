namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestMapping;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("Mapping")]
public class To_A_WijzigBasisgegevensCommand
{
    [Fact]
    public void Then_We_Get_A_WijzigBasisgegevensCommand()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<WijzigBasisgegevensRequest>();

        var actualVCode = fixture.Create<VCode>();
        var actual = request.ToCommand(actualVCode);

        actual.Deconstruct(out var vCode, out var roepnaam, out var korteBeschrijving, out var doelgroep,
                           out var hoofdactiviteitenVerenigingsloket, out var werkingsgebieden);

        vCode.Should().Be(actualVCode);
        roepnaam.Should().Be(request.Roepnaam);
        korteBeschrijving.Should().Be(request.KorteBeschrijving);
        doelgroep.Should().BeEquivalentTo(request.Doelgroep);

        hoofdactiviteitenVerenigingsloket.Should()
                                         .BeEquivalentTo(
                                              request.HoofdactiviteitenVerenigingsloket!.Select(HoofdactiviteitVerenigingsloket.Create));

        werkingsgebieden.Should()
                        .BeEquivalentTo(
                             request.Werkingsgebieden!.Select(Werkingsgebied.Create));
    }
}
