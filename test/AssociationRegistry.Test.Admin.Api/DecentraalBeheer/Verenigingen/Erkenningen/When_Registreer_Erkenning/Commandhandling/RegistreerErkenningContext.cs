namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;

public class RegistreerErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    public Fixture Fixture { get; }
    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; }
    private readonly RegistreerErkenningCommandHandler _commandHandler;
    public RegistreerErkenningCommand RegistreerErkenningCommand { get; private set; }

    public RegistreerErkenningContext(TScenario scenario)
    {
        Fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        AggregateSessionMock = new AggregateSessionMock(Scenario.GetVerenigingState());
        _commandHandler = new RegistreerErkenningCommandHandler(AggregateSessionMock);
        Metadata = Fixture.Create<CommandMetadata>();
        RegistreerErkenningCommand = CreateCommand();
    }

    private RegistreerErkenningCommand CreateCommand() =>
        RegistreerErkenningCommand = Fixture.Create<RegistreerErkenningCommand>() with
        {
            VCode = Scenario.VCode,
            Erkenning = Fixture.Create<TeRegistrerenErkenning>(),
        };

    public IpdcProduct CreateIpdcProduct(string? nummer = null) =>
        Fixture.Create<IpdcProduct>() with
        {
            Nummer = nummer ?? Fixture.Create<string>(),
        };

    public GegevensInitiator CreateInitiator(string? ovoCode = null) =>
        Fixture.Create<GegevensInitiator>() with
        {
            OvoCode = ovoCode ?? Fixture.Create<string>(),
        };

    public CommandMetadata CreateMetadata(string? initiator = null) =>
        Fixture.Create<CommandMetadata>() with
        {
            Initiator = initiator ?? Fixture.Create<string>(),
        };

    public async ValueTask Handle(
        RegistreerErkenningCommand command,
        IpdcProduct ipdcProduct,
        GegevensInitiator initiator,
        CommandMetadata? metadata = null
    ) =>
        await _commandHandler.Handle(
            new CommandEnvelope<RegistreerErkenningCommand>(command, metadata ?? Metadata),
            ipdcProduct,
            initiator
        );

    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd(
        RegistreerErkenningCommand command,
        int erkenningId,
        IpdcProduct ipdcProduct,
        GegevensInitiator initiator
    ) =>
        new(
            ErkenningId: erkenningId,
            ipdcProduct,
            command.Erkenning.ErkenningsPeriode.Startdatum,
            command.Erkenning.ErkenningsPeriode.Einddatum,
            command.Erkenning.Hernieuwingsdatum.Value,
            command.Erkenning.HernieuwingsUrl.Value,
            initiator,
            ErkenningStatus
                .Bepaal(
                    ErkenningsPeriode.Create(
                        command.Erkenning.ErkenningsPeriode.Startdatum,
                        command.Erkenning.ErkenningsPeriode.Einddatum
                    ),
                    DateOnly.FromDateTime(DateTime.Now)
                )
                .Value
        );
}
