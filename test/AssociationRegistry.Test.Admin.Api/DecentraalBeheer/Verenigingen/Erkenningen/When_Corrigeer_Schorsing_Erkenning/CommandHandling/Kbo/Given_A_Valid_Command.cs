namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Schorsing_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerSchorsingErkenning;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.SchorsErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class Given_A_Valid_Command
{
    private readonly CorrigeerSchorsingErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_A_Valid_Command()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new CorrigeerSchorsingErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_It_Saves_An_ErkenningWerdGeschorst_Event()
    {
        var teSchorsenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var command = _fixture.Create<CorrigeerSchorsingErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeCorrigerenSchorsingErkenning>() with { ErkenningId = teSchorsenErkenningId },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        await _commandHandler.Handle(new CommandEnvelope<CorrigeerSchorsingErkenningCommand>(command, commandMetadata));

        _verenigingRepositoryMock.ShouldHaveSavedExact(
            new ErkenningRedenVanSchorsingWerdGecorrigeerd(
                command.Erkenning.ErkenningId,
                command.Erkenning.RedenSchorsing
            )
        );
    }
}
