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
    private readonly RegistreerErkenningCommandHandler _commandHandler;
    private IpdcProduct _ipdcProduct;
    private GegevensInitiator _initiator;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public CommandMetadata Metadata { get; private set; }
    public RegistreerErkenningCommand Command { get; private set; } = null!;

    public RegistreerErkenningContext(TScenario scenario)
    {
        Fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        AggregateSessionMock = new AggregateSessionMock(scenario.GetVerenigingState());
        _commandHandler = new RegistreerErkenningCommandHandler(AggregateSessionMock);
        _ipdcProduct = Fixture.Create<IpdcProduct>();
        _initiator = Fixture.Create<GegevensInitiator>();
        Metadata = Fixture.Create<CommandMetadata>();
    }

    public static RegistreerErkenningContext<TScenario> Given(TScenario scenario) => new(scenario);

    public RegistreerErkenningContext<TScenario> WithCommand(
        Func<RegistreerErkenningCommand, RegistreerErkenningCommand> configure
    )
    {
        Command = CreateCommand();
        Command = configure(Command);

        return this;
    }

    public RegistreerErkenningContext<TScenario> WithActieveErkenning() =>
        WithCommand(cmd =>
            cmd with
            {
                Erkenning = cmd.Erkenning with
                {
                    ErkenningsPeriode = ErkenningsPeriode.Create(startdatum: null, einddatum: null),
                },
            }
        );

    public RegistreerErkenningContext<TScenario> WithNietActieveErkenning()
    {
        var startdatumInFuture = DateOnly.FromDateTime(DateTime.Now.AddDays(Fixture.Create<int>()));

        return WithCommand(cmd =>
            cmd with
            {
                Erkenning = cmd.Erkenning with
                {
                    ErkenningsPeriode = ErkenningsPeriode.Create(startdatumInFuture, einddatum: null),
                },
            }
        );
    }

    public RegistreerErkenningContext<TScenario> WithIpdcProduct(string? nummer = null)
    {
        _ipdcProduct = Fixture.Create<IpdcProduct>() with { Nummer = nummer ?? Fixture.Create<string>() };

        return this;
    }

    public RegistreerErkenningContext<TScenario> WithInitiator(string? ovoCode = null)
    {
        _initiator = Fixture.Create<GegevensInitiator>() with { OvoCode = ovoCode ?? Fixture.Create<string>() };

        return this;
    }

    public RegistreerErkenningContext<TScenario> WithMetadata(string? initiator = null)
    {
        Metadata = Fixture.Create<CommandMetadata>() with { Initiator = initiator ?? Fixture.Create<string>() };

        return this;
    }

    public async ValueTask<RegistreerErkenningContext<TScenario>> WhenHandled()
    {
        await _commandHandler.Handle(
            new CommandEnvelope<RegistreerErkenningCommand>(Command, Metadata),
            _ipdcProduct,
            _initiator
        );

        return this;
    }

    public void ShouldHaveSaved(params IEvent[] events) => AggregateSessionMock.ShouldHaveSavedExact(events);

    public ErkenningWerdGeregistreerd ExpectedErkenningWerdGeregistreerd() =>
        new(
            ErkenningId: VolgendeErkenningId(),
            _ipdcProduct,
            Command.Erkenning.ErkenningsPeriode.Startdatum,
            Command.Erkenning.ErkenningsPeriode.Einddatum,
            Command.Erkenning.Hernieuwingsdatum.Value,
            Command.Erkenning.HernieuwingsUrl.Value,
            _initiator,
            ErkenningStatus
                .Bepaal(
                    ErkenningsPeriode.Create(
                        Command.Erkenning.ErkenningsPeriode.Startdatum,
                        Command.Erkenning.ErkenningsPeriode.Einddatum
                    ),
                    DateOnly.FromDateTime(DateTime.Now)
                )
                .Value
        );

    private int VolgendeErkenningId()
    {
        var erkenningen = Scenario.GetVerenigingState().Erkenningen;

        return erkenningen.Count == 0 ? Erkenningen.InitialId : erkenningen.Max(e => e.ErkenningId) + 1;
    }

    private RegistreerErkenningCommand CreateCommand() =>
        Fixture.Create<RegistreerErkenningCommand>() with
        {
            VCode = Scenario.VCode,
            Erkenning = Fixture.Create<TeRegistrerenErkenning>(),
        };
}
