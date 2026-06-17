namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Valideer_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.ValideerBankrekening;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;

public class ValideerBankrekeningnummerContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly Func<TScenario, int> _defaultBankrekeningnummerId;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }

    private readonly ValideerBankrekeningnummerCommandHandler _commandHandler;

    public ValideerBankrekeningnummerContext(TScenario scenario, Func<TScenario, int> defaultBankrekeningnummerId)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultBankrekeningnummerId = defaultBankrekeningnummerId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new ValideerBankrekeningnummerCommandHandler(AggregateSessionMock);
        Metadata = _fixture.Create<CommandMetadata>();
    }

    public ValideerBankrekeningnummerCommand CreateCommand(int? bankrekeningnummerId = null) =>
        _fixture.Create<ValideerBankrekeningnummerCommand>() with
        {
            VCode = Scenario.VCode,
            BankrekeningnummerId = bankrekeningnummerId ?? _defaultBankrekeningnummerId(Scenario),
        };

    public int CreateUnknownBankrekeningnummerId() => _defaultBankrekeningnummerId(Scenario) + _fixture.Create<int>();

    public CommandMetadata CreateMetadata(string? initiator = null) =>
        _fixture.Create<CommandMetadata>() with
        {
            Initiator = initiator ?? _fixture.Create<string>(),
        };

    public async ValueTask Handle(ValideerBankrekeningnummerCommand command, CommandMetadata? metadata = null) =>
        await _commandHandler.Handle(new CommandEnvelope<ValideerBankrekeningnummerCommand>(command, metadata ?? Metadata));
}
