namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.RequestMapping;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using Framework;
using VCodes;
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
        var fixture = new Fixture().CustomizeAll();

        var request = fixture.Create<WijzigBasisgegevensRequest>();

        var actualVCode = fixture.Create<VCode>();
        var actual = request.ToWijzigBasisgegevensCommand(actualVCode);

        actual.Deconstruct(out var vCode, out var naam, out var korteNaam, out var korteBeschrijving, out var startdatum);

        vCode.Should().Be(actualVCode);
        naam.Should().Be(request.Naam);
        korteNaam.Should().Be(request.KorteNaam);
        korteBeschrijving.Should().Be(request.KorteBeschrijving);
        startdatum.Should().Be(request.Startdatum);
    }
}
