namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Valideer_Bankrekeningnummer.Commandhandling;

using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_A_Valid_Bankrekeningnummer
{
    private readonly ValideerBankrekeningnummerContext<BankrekeningnummerWerdToegevoegdScenario> _ctx = new(
        new BankrekeningnummerWerdToegevoegdScenario(),
        s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
    );

    [Fact]
    public async ValueTask Then_It_Saves_A_BankrekeningnummerWerdGewijzigd_Event()
    {
        var command = _ctx.CreateCommand();

        await _ctx.Handle(command);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd(
                _ctx.Scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                _ctx.Metadata.Initiator
            )
        );
    }
}
