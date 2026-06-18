namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;

public class RegistreerErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }

    private readonly RegistreerErkenningCommandHandler _commandHandler;

    public RegistreerErkenningContext(TScenario scenario)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new RegistreerErkenningCommandHandler(AggregateSessionMock);
        Metadata = _fixture.Create<CommandMetadata>();
    }

    public RegistreerErkenningCommand CreateCommand(TeRegistrerenErkenning? erkenning = null, VCode? vCode = null)
        => _fixture.Create<RegistreerErkenningCommand>() with
        {
            VCode = vCode ?? Scenario.VCode,
            Erkenning = erkenning ?? _fixture.Create<TeRegistrerenErkenning>(),
        };

    public IpdcProduct CreateIpdcProduct(string? nummer = null)
        => _fixture.Create<IpdcProduct>() with { Nummer = nummer ?? _fixture.Create<string>() };

    public GegevensInitiator CreateInitiator(string? ovoCode = null)
        => _fixture.Create<GegevensInitiator>() with { OvoCode = ovoCode ?? _fixture.Create<string>() };

    public CommandMetadata CreateMetadata(string? initiator = null)
        => _fixture.Create<CommandMetadata>() with { Initiator = initiator ?? _fixture.Create<string>() };

    public async ValueTask Handle(
        RegistreerErkenningCommand command,
        IpdcProduct ipdcProduct,
        GegevensInitiator initiator,
        CommandMetadata? metadata = null)
        => await _commandHandler.Handle(
            new CommandEnvelope<RegistreerErkenningCommand>(command, metadata ?? Metadata),
            ipdcProduct,
            initiator);
}
