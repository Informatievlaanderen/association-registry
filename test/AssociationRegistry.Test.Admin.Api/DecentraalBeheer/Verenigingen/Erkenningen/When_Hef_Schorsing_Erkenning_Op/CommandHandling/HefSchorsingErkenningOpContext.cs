namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.HefSchorsingErkenningOp;
using AssociationRegistry.Framework;
using AssociationRegistry.Wegwijs;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Common.StubsMocksFakes.Wegwijs;
using Events;

public class HefSchorsingErkenningOpContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    public Fixture Fixture { get; }
    private readonly Func<TScenario, int> _defaultErkenningId;
    private readonly HefSchorsingErkenningOpCommandHandler _commandHandler;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; private set; }
    public IOrganisatieBevoegdheidServiceMockStub OrganisatieBevoegdheidService { get; }
    public HefSchorsingErkenningOpCommand Command { get; private set; } = null!;

    public HefSchorsingErkenningOpContext(
        TScenario scenario,
        Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null
    )
    {
        Fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultErkenningId = defaultErkenningId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new HefSchorsingErkenningOpCommandHandler(AggregateSessionMock);
        OrganisatieBevoegdheidService = new IOrganisatieBevoegdheidServiceMockStub();
        Metadata = defaultInitiator is not null
            ? Fixture.Create<CommandMetadata>() with
            {
                Initiator = defaultInitiator(Scenario),
            }
            : Fixture.Create<CommandMetadata>();
    }

    public static HefSchorsingErkenningOpContext<TScenario> Given(
        TScenario scenario,
        Func<TScenario, int> erkenningIdSelector,
        Func<TScenario, string>? defaultInitiator = null
    ) => new(scenario, erkenningIdSelector, defaultInitiator);

    public HefSchorsingErkenningOpContext<TScenario> WithCommand(
        Func<HefSchorsingErkenningOpCommand, HefSchorsingErkenningOpCommand> configure
    )
    {
        Command = CreateCommand();
        Command = configure(Command);

        return this;
    }

    public HefSchorsingErkenningOpContext<TScenario> WithInitiator(string? ovoCode = null)
    {
        Metadata = Metadata with { Initiator = ovoCode ?? Fixture.Create<string>() };

        return this;
    }

    public async ValueTask<HefSchorsingErkenningOpContext<TScenario>> WhenHandled(
        IOrganisatieBevoegdheidService? service = null
    )
    {
        await _commandHandler.Handle(
            new CommandEnvelope<HefSchorsingErkenningOpCommand>(Command, Metadata),
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

    private HefSchorsingErkenningOpCommand CreateCommand() =>
        Fixture.Create<HefSchorsingErkenningOpCommand>() with
        {
            VCode = Scenario.VCode,
            ErkenningId = _defaultErkenningId(Scenario),
        };
}
