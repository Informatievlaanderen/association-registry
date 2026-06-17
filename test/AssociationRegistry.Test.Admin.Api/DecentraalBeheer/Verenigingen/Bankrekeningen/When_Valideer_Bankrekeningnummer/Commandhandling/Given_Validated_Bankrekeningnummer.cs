namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Valideer_Bankrekeningnummer.Commandhandling;

using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Xunit;

public class Given_Validated_Bankrekeningnummer
{
    private readonly ValideerBankrekeningnummerContext<BankrekeningnummerWerdGevalideerdScenario> _ctx = new(
        new BankrekeningnummerWerdGevalideerdScenario(),
        s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
    );

    [Fact]
    public async ValueTask Then_Nothing()
    {
        var command = _ctx.CreateCommand();
        var metadata = _ctx.CreateMetadata(
            initiator: _ctx.Scenario.AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd.BevestigdDoor
        );

        await _ctx.Handle(command, metadata);

        _ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
