namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens
    .RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Primitives;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Framework;
using FluentAssertions;
using Moq;
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
        var werkingsgebiedenService = new WerkingsgebiedenServiceMock();

        var actualVCode = fixture.Create<VCode>();
        var actual = request.ToCommand(actualVCode, werkingsgebiedenService);

        actual.Deconstruct(
            out var vCode,
            out var naam,
            out var korteNaam,
            out var korteBeschrijving,
            out var startdatum,
            out var doelgroep,
            out var hoofdactiviteitenVerenigingsloket,
            out var werkingsgebieden,
            out var isUitgeschrevenUitPubliekeDatastroom);

        vCode.Should().Be(actualVCode);
        naam!.ToString().Should().Be(request.Naam);
        korteNaam.Should().Be(request.KorteNaam);
        korteBeschrijving.Should().Be(request.KorteBeschrijving);

        startdatum.Should().Be(NullOrEmpty<Datum>.Create(Datum.Create(request.Startdatum!.Value)));

        doelgroep.Should().BeEquivalentTo(request.Doelgroep);

        hoofdactiviteitenVerenigingsloket.Should()
                                         .BeEquivalentTo(
                                              request.HoofdactiviteitenVerenigingsloket!.Select(HoofdactiviteitVerenigingsloket.Create));

        werkingsgebieden.Should()
                        .BeEquivalentTo(
                             request.Werkingsgebieden!.Select(werkingsgebiedenService.Create));

        isUitgeschrevenUitPubliekeDatastroom.Should().Be(request.IsUitgeschrevenUitPubliekeDatastroom);
    }

    [Fact]
    public void Without_Doelgroep_Then_We_Get_A_WijzigBasisgegevensCommand_With_Doelgroep_Null()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var werkingsgebiedenService = new WerkingsgebiedenServiceMock();

        var request = fixture.Create<WijzigBasisgegevensRequest>();
        request.Doelgroep = null;

        var actualVCode = fixture.Create<VCode>();
        var actual = request.ToCommand(actualVCode, werkingsgebiedenService);

        actual.Deconstruct(
            out var vCode,
            out var naam,
            out var korteNaam,
            out var korteBeschrijving,
            out var startdatum,
            out var doelgroep,
            out var hoofdactiviteitenVerenigingsloket,
            out var werkingsgebieden,
            out var isUitgeschrevenUitPubliekeDatastroom);

        vCode.Should().Be(actualVCode);
        naam!.ToString().Should().Be(request.Naam);
        korteNaam.Should().Be(request.KorteNaam);
        korteBeschrijving.Should().Be(request.KorteBeschrijving);

        startdatum.Should().Be(NullOrEmpty<Datum>.Create(Datum.Create(request.Startdatum!.Value)));

        doelgroep.Should().BeNull();

        hoofdactiviteitenVerenigingsloket.Should()
                                         .BeEquivalentTo(
                                              request.HoofdactiviteitenVerenigingsloket!.Select(HoofdactiviteitVerenigingsloket.Create));

        werkingsgebieden.Should()
                        .BeEquivalentTo(
                             request.Werkingsgebieden!.Select(werkingsgebiedenService.Create));

        isUitgeschrevenUitPubliekeDatastroom.Should().Be(request.IsUitgeschrevenUitPubliekeDatastroom);
    }
}
