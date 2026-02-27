namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Valideer_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.ValideerBankrekening;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class Given_A_KBO_Vereniging
{
    private readonly ValideerBankrekeningnummerCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_A_KBO_Vereniging()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new ValideerBankrekeningnummerCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_BankrekeningnummerWerdGevalideerd_Is_Saved()
    {
        var bankrekeningnummerWerdToegevoegd = _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1;

        var command = _fixture.Create<ValideerBankrekeningnummerCommand>() with
        {
            VCode = _scenario.VCode,
            BankrekeningnummerId = bankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>();

        await _commandHandler.Handle(new CommandEnvelope<ValideerBankrekeningnummerCommand>(command, commandMetadata));

        _aggregateSessionMock.ShouldHaveSavedExact(
            new AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd(
                bankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                commandMetadata.Initiator
            )
        );
    }
}
