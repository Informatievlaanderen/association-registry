namespace AssociationRegistry.Test.ValueObjects.When_Parsing_A_RelatieaanvraagId;

using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Relatieaanvragen.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_Value
{
    [Theory]
    [InlineData("RA0000001")]
    [InlineData("ra0000001")]
    [InlineData(" RA0000001 ")]
    public void With_RA_And_7_Digits_Then_It_Is_Parsed(string value) =>
        RelatieaanvraagId.Parse(value).Should().Be(RelatieaanvraagId.Create(1));

    [Theory]
    [InlineData("RA000001")]
    [InlineData("RA00000001")]
    [InlineData("V0000001")]
    [InlineData("RA000000A")]
    [InlineData("")]
    [InlineData(null)]
    public void With_Another_Format_Then_It_Throws_RelatieaanvraagIdIsOngeldig(string? value)
    {
        var parse = () => RelatieaanvraagId.Parse(value);

        parse
            .Should()
            .Throw<RelatieaanvraagIdIsOngeldig>()
            .WithMessage("Formaat van de aanvraagId moet 'RA0000000' zijn.");
    }
}
