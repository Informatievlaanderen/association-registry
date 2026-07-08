namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;

public class ActiveerErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    public Fixture Fixture { get; }
    private readonly Func<TScenario, int> _defaultErkenningId;
    private readonly ActiveerErkenningCommandHandler _commandHandler;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; private set; }
    public ActiveerErkenningCommand Command { get; private set; } = null!;

    public ActiveerErkenningContext(
        TScenario scenario,
        Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null
    )
    {
        Fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultErkenningId = defaultErkenningId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState(), expectedLoadingDubbel: true);
        _commandHandler = new ActiveerErkenningCommandHandler(AggregateSessionMock);
        Metadata = defaultInitiator is not null
            ? Fixture.Create<CommandMetadata>() with
            {
                Initiator = defaultInitiator(Scenario),
            }
            : Fixture.Create<CommandMetadata>();
    }

    public static ActiveerErkenningContext<TScenario> Given(
        TScenario scenario,
        Func<TScenario, int> erkenningIdSelector,
        Func<TScenario, string>? defaultInitiator = null
    ) => new(scenario, erkenningIdSelector, defaultInitiator);

    public ActiveerErkenningContext<TScenario> WithCommand(
        Func<ActiveerErkenningCommand, ActiveerErkenningCommand> configure
    )
    {
        Command = CreateCommand();
        Command = configure(Command);

        return this;
    }

    public ActiveerErkenningContext<TScenario> WithInitiator(string? ovoCode = null)
    {
        Metadata = Metadata with { Initiator = ovoCode ?? Fixture.Create<string>() };

        return this;
    }

    public async ValueTask<ActiveerErkenningContext<TScenario>> WhenHandled()
    {
        await _commandHandler.Handle(new CommandEnvelope<ActiveerErkenningCommand>(Command, Metadata));

        return this;
    }

    public void ShouldHaveSaved(params IEvent[] events) => AggregateSessionMock.ShouldHaveSavedExact(events);

    public int CreateUnknownErkenningId() => _defaultErkenningId(Scenario) + Fixture.Create<int>();

    public CommandMetadata CreateMetadata(string? initiator = null) =>
        Fixture.Create<CommandMetadata>() with
        {
            Initiator = initiator ?? Fixture.Create<string>(),
        };

    public async ValueTask Handle(ActiveerErkenningCommand command, CommandMetadata? metadata = null) =>
        await _commandHandler.Handle(new CommandEnvelope<ActiveerErkenningCommand>(command, metadata ?? Metadata));

    private ActiveerErkenningCommand CreateCommand() =>
        Fixture.Create<ActiveerErkenningCommand>() with
        {
            VCode = Scenario.VCode,
            ErkenningId = _defaultErkenningId(Scenario),
        };
}
