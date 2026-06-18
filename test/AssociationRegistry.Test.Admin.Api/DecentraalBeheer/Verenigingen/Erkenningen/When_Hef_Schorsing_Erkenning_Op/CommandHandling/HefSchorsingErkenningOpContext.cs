namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.HefSchorsingErkenningOp;
using AssociationRegistry.Framework;
using AssociationRegistry.Wegwijs;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Common.StubsMocksFakes.Wegwijs;

public class HefSchorsingErkenningOpContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly Func<TScenario, int> _defaultErkenningId;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }
    public IOrganisatieBevoegdheidServiceMockStub OrganisatieBevoegdheidService { get; }

    private readonly HefSchorsingErkenningOpCommandHandler _commandHandler;

    public HefSchorsingErkenningOpContext(
        TScenario scenario,
        Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultErkenningId = defaultErkenningId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new HefSchorsingErkenningOpCommandHandler(AggregateSessionMock);
        OrganisatieBevoegdheidService = new IOrganisatieBevoegdheidServiceMockStub();
        Metadata = defaultInitiator is not null
            ? _fixture.Create<CommandMetadata>() with { Initiator = defaultInitiator(Scenario) }
            : _fixture.Create<CommandMetadata>();
    }

    public HefSchorsingErkenningOpCommand CreateCommand(int? erkenningId = null)
        => _fixture.Create<HefSchorsingErkenningOpCommand>() with
        {
            VCode = Scenario.VCode,
            ErkenningId = erkenningId ?? _defaultErkenningId(Scenario),
        };

    public int CreateUnknownErkenningId() => _defaultErkenningId(Scenario) + _fixture.Create<int>();

    public CommandMetadata CreateMetadata(string? initiator = null)
        => _fixture.Create<CommandMetadata>() with { Initiator = initiator ?? _fixture.Create<string>() };

    public async ValueTask Handle(
        HefSchorsingErkenningOpCommand command,
        CommandMetadata? metadata = null,
        IOrganisatieBevoegdheidService? service = null)
        => await _commandHandler.Handle(
            new CommandEnvelope<HefSchorsingErkenningOpCommand>(command, metadata ?? Metadata),
            service ?? OrganisatieBevoegdheidService.Object);
}
