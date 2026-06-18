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

public class SchorsErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly Func<TScenario, int> _defaultErkenningId;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }
    public IOrganisatieBevoegdheidServiceMockStub OrganisatieBevoegdheidService { get; }

    private readonly SchorsErkenningCommandHandler _commandHandler;

    public SchorsErkenningContext(
        TScenario scenario,
        Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultErkenningId = defaultErkenningId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new SchorsErkenningCommandHandler(AggregateSessionMock);
        OrganisatieBevoegdheidService = new IOrganisatieBevoegdheidServiceMockStub();
        Metadata = defaultInitiator is not null
            ? _fixture.Create<CommandMetadata>() with { Initiator = defaultInitiator(Scenario) }
            : _fixture.Create<CommandMetadata>();
    }

    public SchorsErkenningCommand CreateCommand(int? erkenningId = null, string? redenSchorsing = null)
    {
        var erkenning = _fixture.Create<TeSchorsenErkenning>() with
        {
            ErkenningId = erkenningId ?? _defaultErkenningId(Scenario),
            RedenSchorsing = redenSchorsing ?? _fixture.Create<string>(),
        };

        return _fixture.Create<SchorsErkenningCommand>() with
        {
            VCode = Scenario.VCode,
            Erkenning = erkenning,
        };
    }

    public int CreateUnknownErkenningId() => _defaultErkenningId(Scenario) + _fixture.Create<int>();

    public CommandMetadata CreateMetadata(string? initiator = null)
        => _fixture.Create<CommandMetadata>() with { Initiator = initiator ?? _fixture.Create<string>() };

    public async ValueTask Handle(
        SchorsErkenningCommand command,
        CommandMetadata? metadata = null,
        IOrganisatieBevoegdheidService? service = null)
        => await _commandHandler.Handle(
            new CommandEnvelope<SchorsErkenningCommand>(command, metadata ?? Metadata),
            service ?? OrganisatieBevoegdheidService.Object);
}
