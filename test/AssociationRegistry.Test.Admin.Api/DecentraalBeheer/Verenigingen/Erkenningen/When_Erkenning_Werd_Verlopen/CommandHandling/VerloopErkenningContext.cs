namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Erkenning_Werd_Verlopen.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerloopErkenning;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;

public class VerloopErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly Func<TScenario, int> _defaultErkenningId;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }

    private readonly VerloopErkenningCommandHandler _commandHandler;
    public VerloopErkenningCommand VerloopErkenningCommand { get; private set; }

    public VerloopErkenningContext(
        TScenario scenario,
        Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultErkenningId = defaultErkenningId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState(), expectedLoadingDubbel: true);
        _commandHandler = new VerloopErkenningCommandHandler(AggregateSessionMock);
        Metadata = defaultInitiator is not null
            ? _fixture.Create<CommandMetadata>() with
            {
                Initiator = defaultInitiator(Scenario),
            }
            : _fixture.Create<CommandMetadata>();
        VerloopErkenningCommand = CreateCommand();
    }

    private VerloopErkenningCommand CreateCommand()
        => VerloopErkenningCommand = _fixture.Create<VerloopErkenningCommand>() with
        {
            VCode = Scenario.VCode,
            ErkenningId = _defaultErkenningId(Scenario),
        };

    public int CreateUnknownErkenningId() => _defaultErkenningId(Scenario) + _fixture.Create<int>();

    public CommandMetadata CreateMetadata(string? initiator = null)
        => _fixture.Create<CommandMetadata>() with
        {
            Initiator = initiator ?? _fixture.Create<string>(),
        };

    public async ValueTask Handle(VerloopErkenningCommand command, CommandMetadata? metadata = null)
        => await _commandHandler.Handle(
            new CommandEnvelope<VerloopErkenningCommand>(command, metadata ?? Metadata));
}
