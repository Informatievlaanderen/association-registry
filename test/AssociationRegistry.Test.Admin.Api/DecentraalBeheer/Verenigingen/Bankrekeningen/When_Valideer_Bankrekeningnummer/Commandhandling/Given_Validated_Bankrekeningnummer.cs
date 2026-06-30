namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Valideer_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Validated_Bankrekeningnummer
{
    private readonly ValideerBankrekeningnummerContext<BankrekeningnummerWerdGevalideerdScenario> _ctx = new(
        new BankrekeningnummerWerdGevalideerdScenario(),
        s => s.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId
    );

    [Fact]
    public async ValueTask Then_Throws_BankrekeningnummerValidatieIsAlReedsToegevoegd()
    {
        var command = _ctx.CreateCommand();
        var metadata = _ctx.CreateMetadata(
            initiator: _ctx.Scenario.AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd.BevestigdDoor
        );

        var exception = await Assert.ThrowsAsync<BankrekeningnummerValidatieIsAlReedsToegevoegd>(async () =>
            await _ctx.Handle(command, metadata)
        );

        exception
            .Message.Should()
            .Be(string.Format(ExceptionMessages.BankrekeningnummerValidatieIsAlReedsToegevoegd, metadata.Initiator));
    }
}
