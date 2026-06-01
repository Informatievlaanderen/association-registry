namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class Given_Actieve_Erkenning
{
    private readonly ActiveerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Actieve_Erkenning()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new ActiveerErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Saves_An_ErkenningWerdGeactiveerd_Event()
    {
        var teActiverenErkenningId = _scenario.ErkenningWerdGeregistreerdInHuidig.ErkenningId;

        var command = _fixture.Create<ActiveerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            ErkenningId = teActiverenErkenningId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerdInHuidig.GeregistreerdDoor.OvoCode,
        };

        throw new NotImplementedException(); // TODO 202 of error?
        await _commandHandler.Handle(new CommandEnvelope<ActiveerErkenningCommand>(command, commandMetadata));

        _verenigingRepositoryMock.ShouldHaveSavedExact(new ErkenningWerdGeactiveerd(command.ErkenningId));
    }
}
