namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling.Vzer;

using System.Threading.Tasks;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Xunit;

public class Given_Startdatum_Vandaag
{
    private readonly ActiveerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdForActiveerErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Startdatum_Vandaag()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdForActiveerErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new ActiveerErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Saves_An_ErkenningWerdGeactiveerd_Event()
    {
        var teActiverenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var command = _fixture.Create<ActiveerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            ErkenningId = teActiverenErkenningId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        await _commandHandler.Handle(new CommandEnvelope<ActiveerErkenningCommand>(command, commandMetadata));

        _verenigingRepositoryMock.ShouldHaveSavedExact(new ErkenningWerdGeactiveerd(command.ErkenningId));
    }
}
