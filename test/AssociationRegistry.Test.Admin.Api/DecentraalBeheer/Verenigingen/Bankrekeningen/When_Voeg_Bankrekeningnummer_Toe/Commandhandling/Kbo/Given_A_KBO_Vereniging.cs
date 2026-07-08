namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Voeg_Bankrekeningnummer_Toe.Commandhandling.Kbo;

using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_KBO_Vereniging
{
    private readonly VoegBankrekeningnummerToeContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario());

    [Fact]
    public async ValueTask Then_Throws()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new BankrekeningnummerWerdToegevoegd(
                1,
                command.Bankrekeningnummer.Iban.Value,
                command.Bankrekeningnummer.Doel,
                command.Bankrekeningnummer.Titularissen.Value
            )
        );
    }
}
