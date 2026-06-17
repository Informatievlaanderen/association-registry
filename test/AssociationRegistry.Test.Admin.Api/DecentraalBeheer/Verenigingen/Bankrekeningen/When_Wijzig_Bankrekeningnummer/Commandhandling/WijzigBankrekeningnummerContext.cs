namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Wijzig_Bankrekeningnummer.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.WijzigBankrekening;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;

public class WijzigBankrekeningnummerContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly Func<TScenario, int> _defaultBankrekeningnummerId;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }

    private readonly WijzigBankrekeningnummerCommandHandler _commandHandler;

    public WijzigBankrekeningnummerContext(TScenario scenario, Func<TScenario, int> defaultBankrekeningnummerId)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultBankrekeningnummerId = defaultBankrekeningnummerId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new WijzigBankrekeningnummerCommandHandler(AggregateSessionMock);
        Metadata = _fixture.Create<CommandMetadata>();
    }

    public WijzigBankrekeningnummerCommand CreateCommand(
        Func<TeWijzigenBankrekeningnummer, TeWijzigenBankrekeningnummer>? modifier = null)
    {
        var bankrekeningnummer = _fixture.Create<TeWijzigenBankrekeningnummer>() with
        {
            BankrekeningnummerId = _defaultBankrekeningnummerId(Scenario),
        };

        return _fixture.Create<WijzigBankrekeningnummerCommand>() with
        {
            VCode = Scenario.VCode,
            Bankrekeningnummer = modifier?.Invoke(bankrekeningnummer) ?? bankrekeningnummer,
        };
    }

    public CommandMetadata CreateMetadata(string? initiator = null)
        => _fixture.Create<CommandMetadata>() with
        {
            Initiator = initiator ?? _fixture.Create<string>(),
        };

    public async ValueTask Handle(WijzigBankrekeningnummerCommand command, CommandMetadata? metadata = null)
        => await _commandHandler.Handle(
            new CommandEnvelope<WijzigBankrekeningnummerCommand>(command, metadata ?? Metadata));
}
