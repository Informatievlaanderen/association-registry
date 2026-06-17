namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Voeg_Bankrekeningnummer_Toe.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;

public class VoegBankrekeningnummerToeContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }

    private readonly VoegBankrekeningnummerToeCommandHandler _commandHandler;

    public VoegBankrekeningnummerToeContext(TScenario scenario)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new VoegBankrekeningnummerToeCommandHandler(AggregateSessionMock);
        Metadata = _fixture.Create<CommandMetadata>();
    }

    public VoegBankrekeningnummerToeCommand CreateCommand(IbanNummer? iban = null)
    {
        var bankrekeningnummer = iban is null
            ? _fixture.Create<ToeTevoegenBankrekeningnummer>()
            : _fixture.Create<ToeTevoegenBankrekeningnummer>() with { Iban = iban };

        return _fixture.Create<VoegBankrekeningnummerToeCommand>() with
        {
            VCode = Scenario.VCode,
            Bankrekeningnummer = bankrekeningnummer,
        };
    }

    public CommandMetadata CreateMetadata(string? initiator = null)
        => _fixture.Create<CommandMetadata>() with
        {
            Initiator = initiator ?? _fixture.Create<string>(),
        };

    public async ValueTask Handle(VoegBankrekeningnummerToeCommand command, CommandMetadata? metadata = null)
        => await _commandHandler.Handle(
            new CommandEnvelope<VoegBankrekeningnummerToeCommand>(command, metadata ?? Metadata));
}
