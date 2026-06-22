namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Integrations.Grar.Bewaartermijnen;
using Moq;
using Wolverine.Marten;

public class VoegVertegenwoordigerToeContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly BewaartermijnOptions _bewaartermijnOptions;
    private readonly VoegVertegenwoordigerToeCommandHandler _commandHandler;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public Mock<IMartenOutbox> OutboxMock { get; }
    public CommandMetadata Metadata { get; }

    public VoegVertegenwoordigerToeContext(TScenario scenario)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new VoegVertegenwoordigerToeCommandHandler(AggregateSessionMock);
        OutboxMock = new Mock<IMartenOutbox>();
        _bewaartermijnOptions = _fixture.Create<BewaartermijnOptions>();
        Metadata = _fixture.Create<CommandMetadata>();
    }

    public VoegVertegenwoordigerToeCommand CreateCommand(Vertegenwoordiger? vertegenwoordiger = null)
        => new(Scenario.VCode, vertegenwoordiger ?? _fixture.Create<Vertegenwoordiger>());

    public VoegVertegenwoordigerToeCommand CreatePrimairCommand()
        => CreateCommand(_fixture.Create<Vertegenwoordiger>() with { IsPrimair = true });

    public async ValueTask<EntityCommandResult> Handle(
        VoegVertegenwoordigerToeCommand command,
        PersoonUitKsz? persoon = null,
        CommandMetadata? metadata = null)
        => await _commandHandler.Handle(
            new CommandEnvelope<VoegVertegenwoordigerToeCommand>(command, metadata ?? Metadata),
            persoon ?? _fixture.Create<PersoonUitKsz>(),
            OutboxMock.Object,
            _bewaartermijnOptions
        );
}
