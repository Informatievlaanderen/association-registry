namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Maak_Validatie_Bankrekeningnummer_Ongedaan.Commandhandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.ValideerBankrekening;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.MaakValidatieBankrekeningOngedaan;
using Events;
using Xunit;

public class Given_Already_Validated_Bankrekeningnummer
{
    private readonly MaakValidatieBankrekeningnummerOngedaanCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly BankrekeningnummerWerdGevalideerdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_Already_Validated_Bankrekeningnummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new BankrekeningnummerWerdGevalideerdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new MaakValidatieBankrekeningnummerOngedaanCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt_Is_Saved()
    {
        var validatieBankrekeningnummerWerdBevestigd = _scenario.AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd;
        var command = _fixture.Create<MaakValidatieBankrekeningnummerOngedaanCommand>() with
        {
            VCode = _scenario.VCode,
            BankrekeningnummerId = validatieBankrekeningnummerWerdBevestigd.BankrekeningnummerId,
        };
        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = validatieBankrekeningnummerWerdBevestigd.BevestigdDoor,
        };
        await _commandHandler.Handle(new CommandEnvelope<MaakValidatieBankrekeningnummerOngedaanCommand>(command, commandMetadata));
        _aggregateSessionMock.ShouldHaveSavedExact(
            new AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt(
                validatieBankrekeningnummerWerdBevestigd.BankrekeningnummerId,
                commandMetadata.Initiator
            )
        );
    }
}
