namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Erkenning_Werd_Verlopen.CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerloopErkenning;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class Given_A_Valid_Command
{
    private readonly VerloopErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithTeVerlopenErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_A_Valid_Command()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithTeVerlopenErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new VerloopErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_It_Adds_An_ErkenningWerdVerlopen_Event()
    {
        var teVerlopenErkenningId = _scenario.ErkenningWerdGeregistreerdTeVerlopen.ErkenningId;

        var command = _fixture.Create<VerloopErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            ErkenningId = teVerlopenErkenningId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerdTeVerlopen.GeregistreerdDoor.OvoCode,
        };

        await _commandHandler.Handle(new CommandEnvelope<VerloopErkenningCommand>(command, commandMetadata));

        _verenigingRepositoryMock.ShouldHaveSavedExact(new ErkenningWerdVerlopen(command.ErkenningId));
    }
}
