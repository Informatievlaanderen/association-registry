namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Reden_Schorsing_Erkenning.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerRedenSchorsingErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Framework;
using AssociationRegistry.Wegwijs;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Common.StubsMocksFakes.Wegwijs;
using Events;

public class CorrigeerRedenSchorsingErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    public Fixture Fixture { get; }
    private readonly Func<TScenario, int> _defaultErkenningId;
    private readonly CorrigeerRedenSchorsingErkenningCommandHandler _commandHandler;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; private set; }
    public IOrganisatieBevoegdheidServiceMockStub OrganisatieBevoegdheidService { get; }
    public CorrigeerRedenSchorsingErkenningCommand Command { get; private set; } = null!;

    public CorrigeerRedenSchorsingErkenningContext(
        TScenario scenario,
        Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null
    )
    {
        Fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultErkenningId = defaultErkenningId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new CorrigeerRedenSchorsingErkenningCommandHandler(AggregateSessionMock);
        OrganisatieBevoegdheidService = new IOrganisatieBevoegdheidServiceMockStub();
        Metadata = defaultInitiator is not null
            ? Fixture.Create<CommandMetadata>() with
            {
                Initiator = defaultInitiator(Scenario),
            }
            : Fixture.Create<CommandMetadata>();
    }

    public static CorrigeerRedenSchorsingErkenningContext<TScenario> Given(
        TScenario scenario,
        Func<TScenario, int> erkenningIdSelector,
        Func<TScenario, string>? defaultInitiator = null
    ) => new(scenario, erkenningIdSelector, defaultInitiator);

    public CorrigeerRedenSchorsingErkenningContext<TScenario> WithCommand(
        Func<CorrigeerRedenSchorsingErkenningCommand, CorrigeerRedenSchorsingErkenningCommand> configure
    )
    {
        Command = CreateCommand();
        Command = configure(Command);

        return this;
    }

    public CorrigeerRedenSchorsingErkenningContext<TScenario> WithInitiator(string? ovoCode = null)
    {
        Metadata = Metadata with { Initiator = ovoCode ?? Fixture.Create<string>() };

        return this;
    }

    public async ValueTask<CorrigeerRedenSchorsingErkenningContext<TScenario>> WhenHandled(
        IOrganisatieBevoegdheidService? service = null
    )
    {
        await _commandHandler.Handle(
            new CommandEnvelope<CorrigeerRedenSchorsingErkenningCommand>(Command, Metadata),
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

    private CorrigeerRedenSchorsingErkenningCommand CreateCommand() =>
        Fixture.Create<CorrigeerRedenSchorsingErkenningCommand>() with
        {
            VCode = Scenario.VCode,
            Erkenning = Fixture.Create<TeCorrigerenRedenSchorsingErkenning>() with
            {
                ErkenningId = _defaultErkenningId(Scenario),
                RedenSchorsing = Fixture.Create<string>(),
            },
        };
}
