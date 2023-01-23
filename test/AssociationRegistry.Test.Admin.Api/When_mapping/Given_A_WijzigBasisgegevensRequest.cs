namespace AssociationRegistry.Test.Admin.Api.When_mapping;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using AutoFixture;
using FluentAssertions;
using Framework;
using VCodes;
using Xunit;

public class Given_A_WijzigBasisgegevensRequest
{
    [Fact]
    public void Then_We_Get_A_WijzigBasisgegevensCommand()
    {
        var fixture = new Fixture().CustomizeAll();

        var request = fixture.Create<WijzigBasisgegevensRequest>();

        var actualVCode = fixture.Create<VCode>();
        var actual = request.ToWijzigBasisgegevensCommand(actualVCode);

        actual.Deconstruct(out var vCode, out var naam, out var korteNaam, out var korteBeschrijving);

        vCode.Should().Be(actualVCode);
        naam.Should().Be(request.Naam);
        korteNaam.Should().Be(request.KorteNaam);
        korteBeschrijving.Should().Be(request.KorteBeschrijving);
    }
}
