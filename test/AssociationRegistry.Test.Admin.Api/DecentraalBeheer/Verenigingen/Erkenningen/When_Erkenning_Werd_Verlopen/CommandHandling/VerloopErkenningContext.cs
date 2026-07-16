namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Erkenning_Werd_Verlopen.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerloopErkenning;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;

public class VerloopErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    public Fixture Fixture { get; }
    private readonly Func<TScenario, int> _defaultErkenningId;
    private readonly VerloopErkenningCommandHandler _commandHandler;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; private set; }
    public VerloopErkenningCommand Command { get; private set; } = null!;

    public VerloopErkenningContext(
        TScenario scenario,
        Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null
    )
    {
        Fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultErkenningId = defaultErkenningId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState(), expectedLoadingDubbel: true);
        _commandHandler = new VerloopErkenningCommandHandler(AggregateSessionMock);
        Metadata = defaultInitiator is not null
            ? Fixture.Create<CommandMetadata>() with
            {
                Initiator = defaultInitiator(Scenario),
            }
            : Fixture.Create<CommandMetadata>();
    }

    public static VerloopErkenningContext<TScenario> Given(
        TScenario scenario,
        Func<TScenario, int> erkenningIdSelector,
        Func<TScenario, string>? defaultInitiator = null
    ) => new(scenario, erkenningIdSelector, defaultInitiator);

    public VerloopErkenningContext<TScenario> WithCommand(
        Func<VerloopErkenningCommand, VerloopErkenningCommand> configure
    )
    {
        Command = CreateCommand();
        Command = configure(Command);

        return this;
    }

    public VerloopErkenningContext<TScenario> WithInitiator(string? ovoCode = null)
    {
        Metadata = Metadata with { Initiator = ovoCode ?? Fixture.Create<string>() };

        return this;
    }

    public async ValueTask<VerloopErkenningContext<TScenario>> WhenHandled()
    {
        await _commandHandler.Handle(new CommandEnvelope<VerloopErkenningCommand>(Command, Metadata));

        return this;
    }

    public void ShouldHaveSaved(params IEvent[] events) => AggregateSessionMock.ShouldHaveSavedExact(events);

    private VerloopErkenningCommand CreateCommand() =>
        Fixture.Create<VerloopErkenningCommand>() with
        {
            VCode = Scenario.VCode,
            ErkenningId = _defaultErkenningId(Scenario),
        };
}
