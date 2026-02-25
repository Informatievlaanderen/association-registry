namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Valideer_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.ValideerBankrekening;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class Given_A_Valid_Bankrekeningnummer
{
    private readonly ValideerBankrekeningnummerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly BankrekeningnummerWerdToegevoegdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_A_Valid_Bankrekeningnummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new BankrekeningnummerWerdToegevoegdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new ValideerBankrekeningnummerCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_It_Saves_A_BankrekeningnummerWerdGewijzigd_Event()
    {
        var command = _fixture.Create<ValideerBankrekeningnummerCommand>() with
        {
            VCode = _scenario.VCode,
            BankrekeningnummerId = _scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>();
        await _commandHandler.Handle(new CommandEnvelope<ValideerBankrekeningnummerCommand>(command, commandMetadata));

        _aggregateSessionMock.ShouldHaveSavedExact(
            new AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd(
                _scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                commandMetadata.Initiator
            )
        );
    }
}
