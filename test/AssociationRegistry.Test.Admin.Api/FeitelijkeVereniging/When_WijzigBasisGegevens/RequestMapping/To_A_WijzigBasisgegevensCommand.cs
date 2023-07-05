namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging;
using Framework;
using Vereniging;
using AutoFixture;
using FluentAssertions;
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

        actual.Deconstruct(
            out var vCode,
            out var naam,
            out var korteNaam,
            out var korteBeschrijving,
            out var startdatum,
            out var hoofdactiviteitenVerenigingsloket,
            out var isUitgeschrevenUitPubliekeDatastroom);

        vCode.Should().Be(actualVCode);
        naam!.ToString().Should().Be(request.Naam);
        korteNaam.Should().Be(request.KorteNaam);
        korteBeschrijving.Should().Be(request.KorteBeschrijving);
        startdatum.Should().Be(
            request.Startdatum.IsNull ? null :
            request.Startdatum.IsEmpty ? Startdatum.Leeg : Startdatum.Create(request.Startdatum.Value));
        hoofdactiviteitenVerenigingsloket.Should().BeEquivalentTo(request.HoofdactiviteitenVerenigingsloket!.Select(HoofdactiviteitVerenigingsloket.Create));
        isUitgeschrevenUitPubliekeDatastroom.Should().Be(request.IsUitgeschrevenUitPubliekeDatastroom);
    }
}
