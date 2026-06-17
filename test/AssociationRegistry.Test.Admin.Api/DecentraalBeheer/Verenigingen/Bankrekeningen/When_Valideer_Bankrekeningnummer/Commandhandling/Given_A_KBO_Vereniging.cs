namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Valideer_Bankrekeningnummer.Commandhandling;

using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_KBO_Vereniging
{
    private readonly ValideerBankrekeningnummerContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario> _ctx =
        new(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario(),
            s => s.BankrekeningnummerWerdToegevoegdVanuitKBO1.BankrekeningnummerId
        );

    [Fact]
    public async ValueTask Then_BankrekeningnummerWerdGevalideerd_Is_Saved()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd(
                _ctx.Scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1.BankrekeningnummerId,
                _ctx.Metadata.Initiator
            )
        );
    }
}
