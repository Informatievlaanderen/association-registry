namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;

public class RegistreerErkenningTest<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly RegistreerErkenningCommandHandler _handler;

    private RegistreerErkenningCommand _command;
    private IpdcProduct _ipdcProduct;
    private GegevensInitiator _initiator;
    private CommandMetadata _metadata;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }

    private RegistreerErkenningTest(TScenario scenario)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        AggregateSessionMock = new AggregateSessionMock(scenario.GetVerenigingState());
        _handler = new RegistreerErkenningCommandHandler(AggregateSessionMock);
        _command = _fixture.Create<RegistreerErkenningCommand>() with
        {
            VCode = scenario.VCode,
            Erkenning = _fixture.Create<TeRegistrerenErkenning>(),
        };
        _ipdcProduct = _fixture.Create<IpdcProduct>();
        _initiator = _fixture.Create<GegevensInitiator>();
        _metadata = _fixture.Create<CommandMetadata>();
    }

    public static RegistreerErkenningTest<TScenario> Given(TScenario scenario) => new(scenario);

    public RegistreerErkenningTest<TScenario> WithCommand(
        Func<RegistreerErkenningCommand, RegistreerErkenningCommand> configure
    )
    {
        _command = configure(_command);
        return this;
    }

    public RegistreerErkenningTest<TScenario> WithIpdcProduct(string? nummer = null)
    {
        _ipdcProduct = _fixture.Create<IpdcProduct>() with { Nummer = nummer ?? _fixture.Create<string>() };
        return this;
    }

    public RegistreerErkenningTest<TScenario> WithInitiator(string? ovoCode = null)
    {
        _initiator = _fixture.Create<GegevensInitiator>() with { OvoCode = ovoCode ?? _fixture.Create<string>() };
        return this;
    }

    public RegistreerErkenningTest<TScenario> WithMetadata(string? initiator = null)
    {
        _metadata = _fixture.Create<CommandMetadata>() with { Initiator = initiator ?? _fixture.Create<string>() };
        return this;
    }

    public RegistreerErkenningTest<TScenario> WithActieveErkenning() =>
        WithCommand(cmd =>
            cmd with
            {
                Erkenning = cmd.Erkenning with
                {
                    ErkenningsPeriode = ErkenningsPeriode.Create(startdatum: null, einddatum: null),
                },
            }
        );

    public RegistreerErkenningTest<TScenario> WithNietActieveErkenning()
    {
        var startdatumInFuture = DateOnly.FromDateTime(DateTime.Now.AddDays(_fixture.Create<int>()));

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

    public async ValueTask<RegistreerErkenningTest<TScenario>> WhenHandled()
    {
        await _handler.Handle(
            new CommandEnvelope<RegistreerErkenningCommand>(_command, _metadata),
            _ipdcProduct,
            _initiator
        );

        return this;
    }

    public void ShouldHaveSaved(params IEvent[] events) => AggregateSessionMock.ShouldHaveSavedExact(events);

    private int VolgendeErkenningId()
    {
        var erkenningen = Scenario.GetVerenigingState().Erkenningen;

        return erkenningen.Count == 0 ? Erkenningen.InitialId : erkenningen.Max(e => e.ErkenningId) + 1;
    }

    public ErkenningWerdGeregistreerd ExpectedErkenningWerdGeregistreerd() =>
        new(
            ErkenningId: VolgendeErkenningId(),
            _ipdcProduct,
            _command.Erkenning.ErkenningsPeriode.Startdatum,
            _command.Erkenning.ErkenningsPeriode.Einddatum,
            _command.Erkenning.Hernieuwingsdatum.Value,
            _command.Erkenning.HernieuwingsUrl.Value,
            _initiator,
            ErkenningStatus
                .Bepaal(
                    ErkenningsPeriode.Create(
                        _command.Erkenning.ErkenningsPeriode.Startdatum,
                        _command.Erkenning.ErkenningsPeriode.Einddatum
                    ),
                    DateOnly.FromDateTime(DateTime.Now)
                )
                .Value
        );
}
