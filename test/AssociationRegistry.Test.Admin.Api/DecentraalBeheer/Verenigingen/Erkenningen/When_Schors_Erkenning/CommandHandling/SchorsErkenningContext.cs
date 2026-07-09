namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.SchorsErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Framework;
using AssociationRegistry.Wegwijs;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Common.StubsMocksFakes.Wegwijs;
using Events;

public class SchorsErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    public Fixture Fixture { get; }
    private readonly Func<TScenario, int> _defaultErkenningId;
    private readonly SchorsErkenningCommandHandler _commandHandler;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; private set; }
    public IOrganisatieBevoegdheidServiceMockStub OrganisatieBevoegdheidService { get; }
    public SchorsErkenningCommand Command { get; private set; } = null!;

    public SchorsErkenningContext(
        TScenario scenario,
        Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null
    )
    {
        Fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultErkenningId = defaultErkenningId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new SchorsErkenningCommandHandler(AggregateSessionMock);
        OrganisatieBevoegdheidService = new IOrganisatieBevoegdheidServiceMockStub();
        Metadata = defaultInitiator is not null
            ? Fixture.Create<CommandMetadata>() with
            {
                Initiator = defaultInitiator(Scenario),
            }
            : Fixture.Create<CommandMetadata>();
    }

    public static SchorsErkenningContext<TScenario> Given(
        TScenario scenario,
        Func<TScenario, int> erkenningIdSelector,
        Func<TScenario, string>? defaultInitiator = null
    ) => new(scenario, erkenningIdSelector, defaultInitiator);

    public SchorsErkenningContext<TScenario> WithCommand(Func<SchorsErkenningCommand, SchorsErkenningCommand> configure)
    {
        Command = CreateCommand();
        Command = configure(Command);

        return this;
    }

    public SchorsErkenningContext<TScenario> WithInitiator(string? ovoCode = null)
    {
        Metadata = Metadata with { Initiator = ovoCode ?? Fixture.Create<string>() };

        return this;
    }

    public async ValueTask<SchorsErkenningContext<TScenario>> WhenHandled(
        IOrganisatieBevoegdheidService? service = null
    )
    {
        await _commandHandler.Handle(
            new CommandEnvelope<SchorsErkenningCommand>(Command, Metadata),
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

    private SchorsErkenningCommand CreateCommand()
    {
        var erkenning = Fixture.Create<TeSchorsenErkenning>() with { ErkenningId = _defaultErkenningId(Scenario) };

        return Fixture.Create<SchorsErkenningCommand>() with
        {
            VCode = Scenario.VCode,
            Erkenning = erkenning,
        };
    }
}
