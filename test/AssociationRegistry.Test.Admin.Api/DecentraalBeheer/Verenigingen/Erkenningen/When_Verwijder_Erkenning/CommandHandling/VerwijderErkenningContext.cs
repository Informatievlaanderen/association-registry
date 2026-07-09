namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Verwijder_Erkenning.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerwijderErkenning;
using AssociationRegistry.Framework;
using AssociationRegistry.Wegwijs;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Common.StubsMocksFakes.Wegwijs;
using Events;

public class VerwijderErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    public Fixture Fixture { get; }
    private readonly Func<TScenario, int> _defaultErkenningId;
    private readonly VerwijderErkenningCommandHandler _commandHandler;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; private set; }
    public IOrganisatieBevoegdheidServiceMockStub OrganisatieBevoegdheidService { get; }
    public VerwijderErkenningCommand Command { get; private set; } = null!;

    public VerwijderErkenningContext(
        TScenario scenario,
        Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null
    )
    {
        Fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultErkenningId = defaultErkenningId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new VerwijderErkenningCommandHandler(AggregateSessionMock);
        OrganisatieBevoegdheidService = new IOrganisatieBevoegdheidServiceMockStub();
        Metadata = defaultInitiator is not null
            ? Fixture.Create<CommandMetadata>() with
            {
                Initiator = defaultInitiator(Scenario),
            }
            : Fixture.Create<CommandMetadata>();
    }

    public static VerwijderErkenningContext<TScenario> Given(
        TScenario scenario,
        Func<TScenario, int> erkenningIdSelector,
        Func<TScenario, string>? defaultInitiator = null
    ) => new(scenario, erkenningIdSelector, defaultInitiator);

    public VerwijderErkenningContext<TScenario> WithCommand(
        Func<VerwijderErkenningCommand, VerwijderErkenningCommand> configure
    )
    {
        Command = CreateCommand();
        Command = configure(Command);

        return this;
    }

    public VerwijderErkenningContext<TScenario> WithInitiator(string? ovoCode = null)
    {
        Metadata = Metadata with { Initiator = ovoCode ?? Fixture.Create<string>() };

        return this;
    }

    public async ValueTask<VerwijderErkenningContext<TScenario>> WhenHandled(
        IOrganisatieBevoegdheidService? service = null
    )
    {
        await _commandHandler.Handle(
            new CommandEnvelope<VerwijderErkenningCommand>(Command, Metadata),
            service ?? OrganisatieBevoegdheidService.Object
        );

        return this;
    }

    public void ShouldHaveSaved(params IEvent[] events) => AggregateSessionMock.ShouldHaveSavedExact(events);

    public int CreateUnknownErkenningId() => _defaultErkenningId(Scenario) + Fixture.Create<int>();

    public CommandMetadata CreateMetadata(string? initiator = null) =>
        Fixture.Create<CommandMetadata>() with
        {
            Initiator = initiator ?? Fixture.Create<string>(),
        };

    private VerwijderErkenningCommand CreateCommand() =>
        Fixture.Create<VerwijderErkenningCommand>() with
        {
            VCode = Scenario.VCode,
            ErkenningId = _defaultErkenningId(Scenario),
        };
}
