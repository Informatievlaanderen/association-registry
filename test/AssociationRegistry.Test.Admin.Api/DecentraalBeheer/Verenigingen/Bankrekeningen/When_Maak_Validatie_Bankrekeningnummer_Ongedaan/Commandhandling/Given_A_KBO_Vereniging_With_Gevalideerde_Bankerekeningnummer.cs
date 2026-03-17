namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Maak_Validatie_Bankrekeningnummer_Ongedaan.Commandhandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.ValideerBankrekening;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.MaakValidatieBankrekeningOngedaan;
using Xunit;

public class Given_A_KBO_Vereniging_With_Gevalideerde_Bankerekeningnummer
{
    private readonly MaakValidatieBankrekeningnummerOngedaanCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGevalideerdBankrekeningnummersScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_A_KBO_Vereniging_With_Gevalideerde_Bankerekeningnummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGevalideerdBankrekeningnummersScenario();
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
