namespace AssociationRegistry.Test.ValueObjects.When_Parsing_A_RelatieType;

using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Relatieaanvragen.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_An_Unknown_Value
{
    [Theory]
    [InlineData("Onbekend")]
    [InlineData("Lid")]
    [InlineData("")]
    public void Then_It_Throws_RelatieTypeIsOngeldig_With_The_Allowed_Values(string value)
    {
        var parse = () => RelatieType.Parse(value);

        parse
            .Should()
            .Throw<RelatieTypeIsOngeldig>()
            .WithMessage(
                $"RelatieType moet {RelatieType.Labels.Lidmaatschap} of {RelatieType.Labels.Subvereniging} zijn."
            );
    }
}
