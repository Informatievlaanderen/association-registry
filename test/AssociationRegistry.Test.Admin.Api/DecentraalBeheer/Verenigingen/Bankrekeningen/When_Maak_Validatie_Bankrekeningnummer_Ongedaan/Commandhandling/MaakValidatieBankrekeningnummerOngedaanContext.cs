namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Maak_Validatie_Bankrekeningnummer_Ongedaan.Commandhandling;

using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.MaakValidatieBankrekeningOngedaan;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;

public class MaakValidatieBankrekeningnummerOngedaanContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly Func<TScenario, int> _defaultBankrekeningnummerId;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }

    private readonly MaakValidatieBankrekeningnummerOngedaanCommandHandler _commandHandler;

    public MaakValidatieBankrekeningnummerOngedaanContext(
        TScenario scenario,
        Func<TScenario, int> defaultBankrekeningnummerId
    )
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _defaultBankrekeningnummerId = defaultBankrekeningnummerId;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new MaakValidatieBankrekeningnummerOngedaanCommandHandler(AggregateSessionMock);
        Metadata = _fixture.Create<CommandMetadata>();
    }

    public MaakValidatieBankrekeningnummerOngedaanCommand CreateCommand(int? bankrekeningnummerId = null) =>
        _fixture.Create<MaakValidatieBankrekeningnummerOngedaanCommand>() with
        {
            VCode = Scenario.VCode,
            BankrekeningnummerId = bankrekeningnummerId ?? _defaultBankrekeningnummerId(Scenario),
        };

    public int CreateUnknownBankrekeningnummerId() => int.MaxValue;

    public CommandMetadata CreateMetadata(string? initiator = null) =>
        _fixture.Create<CommandMetadata>() with
        {
            Initiator = initiator ?? _fixture.Create<string>(),
        };

    public async ValueTask Handle(
        MaakValidatieBankrekeningnummerOngedaanCommand command,
        CommandMetadata? metadata = null
    ) =>
        await _commandHandler.Handle(
            new CommandEnvelope<MaakValidatieBankrekeningnummerOngedaanCommand>(command, metadata ?? Metadata)
        );
}
