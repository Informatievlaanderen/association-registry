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

public class CorrigeerRedenSchorsingErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly Func<TScenario, int> _defaultErkenningId;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }
    public IOrganisatieBevoegdheidServiceMockStub OrganisatieBevoegdheidService { get; }

    private readonly CorrigeerRedenSchorsingErkenningCommandHandler _commandHandler;

    public CorrigeerRedenSchorsingErkenningContext(
        TScenario scenario,
        Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultErkenningId = defaultErkenningId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new CorrigeerRedenSchorsingErkenningCommandHandler(AggregateSessionMock);
        OrganisatieBevoegdheidService = new IOrganisatieBevoegdheidServiceMockStub();
        Metadata = defaultInitiator is not null
            ? _fixture.Create<CommandMetadata>() with { Initiator = defaultInitiator(Scenario) }
            : _fixture.Create<CommandMetadata>();
    }

    public CorrigeerRedenSchorsingErkenningCommand CreateCommand(string? redenSchorsing = null, int? erkenningId = null)
    {
        var erkenning = _fixture.Create<TeCorrigerenRedenSchorsingErkenning>() with
        {
            ErkenningId = erkenningId ?? _defaultErkenningId(Scenario),
            RedenSchorsing = redenSchorsing ?? _fixture.Create<string>(),
        };

        return _fixture.Create<CorrigeerRedenSchorsingErkenningCommand>() with
        {
            VCode = Scenario.VCode,
            Erkenning = erkenning,
        };
    }

    public int CreateUnknownErkenningId() => _defaultErkenningId(Scenario) + _fixture.Create<int>();

    public CommandMetadata CreateMetadata(string? initiator = null) =>
        _fixture.Create<CommandMetadata>() with
        {
            Initiator = initiator ?? _fixture.Create<string>(),
        };

    public async ValueTask Handle(
        CorrigeerRedenSchorsingErkenningCommand command,
        CommandMetadata? metadata = null,
        IOrganisatieBevoegdheidService? service = null) =>
        await _commandHandler.Handle(
            new CommandEnvelope<CorrigeerRedenSchorsingErkenningCommand>(command, metadata ?? Metadata),
            service ?? OrganisatieBevoegdheidService.Object);
}
