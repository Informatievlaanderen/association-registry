namespace AssociationRegistry.Test.ValueObjects.When_Parsing_A_RelatieaanvraagStatus;

using DecentraalBeheer.Relatieaanvragen;
using DecentraalBeheer.Relatieaanvragen.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_An_Unknown_Value
{
    [Theory]
    [InlineData("Open")]
    [InlineData("")]
    public void Then_It_Throws_RelatieaanvraagStatusIsOngeldig_With_The_Allowed_Values(string value)
    {
        var parse = () => RelatieaanvraagStatus.Parse(value);

        parse
            .Should()
            .Throw<RelatieaanvraagStatusIsOngeldig>()
            .WithMessage("De status moet Aangevraagd, Bevestigd, Geweigerd, Ingetrokken of Vervallen zijn.");
    }
}
