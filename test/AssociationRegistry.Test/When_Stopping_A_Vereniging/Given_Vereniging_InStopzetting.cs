namespace AssociationRegistry.Test.When_Stopping_A_Vereniging;

using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.StopVereniging;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Events;
using Framework;
using Xunit;

public class Given_Vereniging_InStopzetting
{
    private AggregateSessionMock _aggregateSessionMock;
    private VzerWerdInStopzettingGeplaatstScenario _scenario;
    private Fixture _fixture;
    private StopVerenigingCommandHandler _commandHandler;

    public Given_Vereniging_InStopzetting()
    {
        _fixture = new Fixture().CustomizeDomain();

        _scenario = new VzerWerdInStopzettingGeplaatstScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new StopVerenigingCommandHandler(_aggregateSessionMock, new ClockStub(DateOnly.MaxValue));
    }

    [Fact]
    public async Task Then_Saves_VerenigingWerdGestopt_And_VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestopt()
    {
        var cmd = _fixture.Create<StopVerenigingCommand>() with
        {
            VCode = _scenario.VCode,
            Einddatum = Datum.Create(
                _scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Startdatum.Value.AddDays(
                    _fixture.Create<int>()
                )
            ),
        };

        await _commandHandler.Handle(
            new CommandEnvelope<StopVerenigingCommand>(cmd, _fixture.Create<CommandMetadata>())
        );

        _aggregateSessionMock.ShouldHaveSavedExact(
            new VerenigingWerdGestopt(cmd.Einddatum.Value),
            new VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestopt()
        );
    }
}
