namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Activeer_Erkenning.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;

public class ActiveerErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly Func<TScenario, int> _defaultErkenningId;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }

    private readonly ActiveerErkenningCommandHandler _commandHandler;

    public ActiveerErkenningContext(
        TScenario scenario,
        Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultErkenningId = defaultErkenningId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState(), expectedLoadingDubbel: true);
        _commandHandler = new ActiveerErkenningCommandHandler(AggregateSessionMock);
        Metadata = defaultInitiator is not null
            ? _fixture.Create<CommandMetadata>() with { Initiator = defaultInitiator(Scenario) }
            : _fixture.Create<CommandMetadata>();
    }

    public ActiveerErkenningCommand CreateCommand(int? erkenningId = null)
        => _fixture.Create<ActiveerErkenningCommand>() with
        {
            VCode = Scenario.VCode,
            ErkenningId = erkenningId ?? _defaultErkenningId(Scenario),
        };

    public int CreateUnknownErkenningId() => _defaultErkenningId(Scenario) + _fixture.Create<int>();

    public CommandMetadata CreateMetadata(string? initiator = null)
        => _fixture.Create<CommandMetadata>() with { Initiator = initiator ?? _fixture.Create<string>() };

    public async ValueTask Handle(ActiveerErkenningCommand command, CommandMetadata? metadata = null)
        => await _commandHandler.Handle(
            new CommandEnvelope<ActiveerErkenningCommand>(command, metadata ?? Metadata));
}
