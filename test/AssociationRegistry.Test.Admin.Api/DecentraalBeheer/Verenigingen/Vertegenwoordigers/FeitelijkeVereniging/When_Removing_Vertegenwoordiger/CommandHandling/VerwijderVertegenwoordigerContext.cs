namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;

public class VerwijderVertegenwoordigerContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly Func<TScenario, int> _defaultVertegenwoordigerId;
    private readonly VerwijderVertegenwoordigerCommandHandler _commandHandler;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }

    public VerwijderVertegenwoordigerContext(TScenario scenario, Func<TScenario, int> defaultVertegenwoordigerId)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultVertegenwoordigerId = defaultVertegenwoordigerId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new VerwijderVertegenwoordigerCommandHandler(AggregateSessionMock);
        Metadata = _fixture.Create<CommandMetadata>();
    }

    public VerwijderVertegenwoordigerCommand CreateCommand(int? vertegenwoordigerId = null)
        => new(Scenario.VCode, vertegenwoordigerId ?? _defaultVertegenwoordigerId(Scenario));

    public int CreateUnknownVertegenwoordigerId()
        => _defaultVertegenwoordigerId(Scenario) + 1;

    public async Task Handle(VerwijderVertegenwoordigerCommand command, CommandMetadata? metadata = null)
        => await _commandHandler.Handle(
            new CommandEnvelope<VerwijderVertegenwoordigerCommand>(command, metadata ?? Metadata));
}
