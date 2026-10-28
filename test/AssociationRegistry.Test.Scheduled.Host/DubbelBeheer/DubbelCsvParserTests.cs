namespace AssociationRegistry.Test.Scheduled.Host.DubbelBeheer;

using System.Text;
using AssociationRegistry.Scheduled.Host.DubbelBeheer.Parsing;
using FluentAssertions;

public class DubbelCsvParserTests
{
    private static IEnumerable<(string, string)> Parse(string inhoud, bool metBom = false)
    {
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: metBom);

        return DubbelCsvParser
            .Parse(new MemoryStream(encoding.GetPreamble().Concat(encoding.GetBytes(inhoud)).ToArray()))
            .Select(x => (x.DubbeleVCode.Value, x.AuthentiekeVCode.Value));
    }

    [Theory]
    [InlineData("DubbeleVCode;AuthentiekeVCode\nV0001001;V0001002\nV0001003;V0001004\n")]
    [InlineData("DubbeleVCode,AuthentiekeVCode\nV0001001,V0001002\nV0001003,V0001004\n")]
    [InlineData("dubbelevcode;authentiekevcode\n v0001001 ; V0001002 \nV0001003;V0001004\n")]
    public void Leest_alle_rijen_in_volgorde(string inhoud) =>
        Parse(inhoud).Should().Equal(("V0001001", "V0001002"), ("V0001003", "V0001004"));

    [Fact]
    public void Een_bom_wordt_genegeerd() =>
        Parse("DubbeleVCode;AuthentiekeVCode\nV0001001;V0001002\n", metBom: true)
            .Should()
            .Equal(("V0001001", "V0001002"));

    [Fact]
    public void Een_bestand_met_enkel_een_header_levert_geen_rijen_op() =>
        Parse("DubbeleVCode;AuthentiekeVCode\n").Should().BeEmpty();

    [Theory]
    [InlineData("DubbeleVCode;AuthentiekeVCode\nV0001001\n")]
    [InlineData("Foo;Bar\nV0001001;V0001002\n")]
    [InlineData("DubbeleVCode;AuthentiekeVCode\nX0001001;V0001002\n")]
    [InlineData("DubbeleVCode;AuthentiekeVCode\nV0001001;V001\n")]
    [InlineData("DubbeleVCode;AuthentiekeVCode\nV0001001;V0001001\n")]
    public void Een_ongeldig_bestand_faalt(string inhoud)
    {
        var act = () => Parse(inhoud).ToList();

        act.Should().Throw<Exception>();
    }
}
