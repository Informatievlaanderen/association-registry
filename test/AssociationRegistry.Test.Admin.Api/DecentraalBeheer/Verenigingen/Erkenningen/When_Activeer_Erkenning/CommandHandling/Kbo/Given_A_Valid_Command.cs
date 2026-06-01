namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class Given_A_Valid_Command
{
    private readonly ActiveerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_A_Valid_Command()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new ActiveerErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_It_Adds_An_ErkenningWerdGeactiveerd_Event()
    {
        // TODO Testen naar vZER kopieren
        var teActiverenErkenningId = _scenario.ErkenningWerdGeregistreerdTeActiveren.ErkenningId;

        var command = _fixture.Create<ActiveerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            ErkenningId = teActiverenErkenningId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerdTeActiveren.GeregistreerdDoor.OvoCode,
        };

        await _commandHandler.Handle(new CommandEnvelope<ActiveerErkenningCommand>(command, commandMetadata));

        _verenigingRepositoryMock.ShouldHaveSavedExact(new ErkenningWerdGeactiveerd(command.ErkenningId));
    }
}
