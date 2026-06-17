namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_verwijder_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VerwijderBankrekening;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;

public class VerwijderBankrekeningnummerContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly Func<TScenario, int> _defaultBankrekeningnummerId;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }

    private readonly VerwijderBankrekeningnummerCommandHandler _commandHandler;

    public VerwijderBankrekeningnummerContext(TScenario scenario, Func<TScenario, int> defaultBankrekeningnummerId)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultBankrekeningnummerId = defaultBankrekeningnummerId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new VerwijderBankrekeningnummerCommandHandler(AggregateSessionMock);
        Metadata = _fixture.Create<CommandMetadata>();
    }

    public VerwijderBankrekeningnummerCommand CreateCommand(int? bankrekeningnummerId = null)
        => new(
            VCode: Scenario.VCode,
            BankrekeningnummerId: bankrekeningnummerId ?? _defaultBankrekeningnummerId(Scenario)
        );

    public CommandMetadata CreateMetadata(string? initiator = null)
        => _fixture.Create<CommandMetadata>() with
        {
            Initiator = initiator ?? _fixture.Create<string>(),
        };

    public async ValueTask Handle(VerwijderBankrekeningnummerCommand command, CommandMetadata? metadata = null)
        => await _commandHandler.Handle(
            new CommandEnvelope<VerwijderBankrekeningnummerCommand>(command, metadata ?? Metadata));
}
