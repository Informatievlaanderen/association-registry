namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.
    When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Magda.Persoon;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_An_Overleden_Vertegenwoordiger
{
    private readonly VoegVertegenwoordigerToeContext<FeitelijkeVerenigingWerdGeregistreerdScenario> _ctx =
        new(new FeitelijkeVerenigingWerdGeregistreerdScenario());

    [Fact]
    public async ValueTask Then_A_VertegenwoordigerWerdToegevoegd_Event_Is_Saved()
    {
        var command = _ctx.CreateCommand();

        var exception = await Assert.ThrowsAsync<OverledenVertegenwoordigerKanNietToegevoegdWorden>(async () =>
            await _ctx.Handle(
                command,
                new PersoonUitKsz(string.Empty, true)
            )
        );

        exception.Message.Should().Be(ExceptionMessages.OverledenVertegenwoordigerKanNietToegevoegdWorden);
    }
}
