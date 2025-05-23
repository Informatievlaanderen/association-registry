namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Framework;
using FluentAssertions;
using Xunit;

public class To_A_WijzigBasisgegevensCommand
{
    [Fact]
    public void Then_We_Get_A_WijzigBasisgegevensCommand()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var werkingsgebiedenService = new WerkingsgebiedenServiceMock();

        var request = fixture.Create<WijzigBasisgegevensRequest>();

        var actualVCode = fixture.Create<VCode>();
        var actual = request.ToCommand(actualVCode, werkingsgebiedenService);

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
                             request.Werkingsgebieden!.Select(werkingsgebiedenService.Create));
    }
}
