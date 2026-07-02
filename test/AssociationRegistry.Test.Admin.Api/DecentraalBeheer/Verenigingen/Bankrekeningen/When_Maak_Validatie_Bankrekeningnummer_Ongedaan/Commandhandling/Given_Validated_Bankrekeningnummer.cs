namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Maak_Validatie_Bankrekeningnummer_Ongedaan.Commandhandling;

using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using Xunit;

public class Given_Validated_Bankrekeningnummer
{
    private readonly MaakValidatieBankrekeningnummerOngedaanContext<BankrekeningnummerWerdGevalideerdScenario> _ctx =
        new(
            new BankrekeningnummerWerdGevalideerdScenario(),
            s => s.AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd.BankrekeningnummerId
        );

    [Fact]
    public async ValueTask Then_AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt_Is_Saved()
    {
        var command = _ctx.CreateCommand();
        var commandMetadata = _ctx.CreateMetadata(
            initiator: _ctx.Scenario.AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd.BevestigdDoor.OvoCode
        );

        await _ctx.Handle(command, commandMetadata);

        _ctx.AggregateSessionMock.ShouldHaveSavedExact(
            new AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt(
                _ctx.Scenario.AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd.BankrekeningnummerId,
                commandMetadata.Initiator
            )
        );
    }
}
