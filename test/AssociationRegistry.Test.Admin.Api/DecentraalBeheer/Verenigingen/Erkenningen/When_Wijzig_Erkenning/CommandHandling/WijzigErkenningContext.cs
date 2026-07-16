namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.
    CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Framework;
using AssociationRegistry.Wegwijs;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Common.StubsMocksFakes.Wegwijs;
using Events;
using Primitives;

public class WijzigErkenningContext<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    public Fixture Fixture { get; }
    private readonly WijzigErkenningCommandHandler _handler;
    public WijzigErkenningCommand Command;
    private readonly int _erkenningId;
    public CommandMetadata Metadata { get; private set; }
    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public IOrganisatieBevoegdheidServiceMockStub OrganisatieBevoegdheidService { get; }

    public WijzigErkenningContext(
        TScenario scenario,
        Func<TScenario, int> defaultErkenningId,
        Func<TScenario, string>? defaultInitiator = null)
    {
        Fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;

        _erkenningId = defaultErkenningId(scenario);

        AggregateSessionMock = new AggregateSessionMock(scenario.GetVerenigingState());
        _handler = new WijzigErkenningCommandHandler(AggregateSessionMock);

        OrganisatieBevoegdheidService = new IOrganisatieBevoegdheidServiceMockStub();

        Metadata = defaultInitiator is not null
            ? Fixture.Create<CommandMetadata>() with
            {
                Initiator = defaultInitiator(Scenario),
            }
            : Fixture.Create<CommandMetadata>();
    }

    public void WithErkenningCommand(Func<WijzigErkenningCommand, WijzigErkenningCommand> wijzigCommandFunc)
    {
        Command = CreateWijzigErkenningCommand();
        Command = wijzigCommandFunc(Command);
    }

    public static WijzigErkenningContext<TScenario> Given(TScenario scenario, Func<TScenario, int> erkenningIdSelector)
        =>
            new(scenario, erkenningIdSelector);

    public WijzigErkenningContext<TScenario> WithCommand(Func<WijzigErkenningCommand, WijzigErkenningCommand> configure)
    {
        Command = CreateWijzigErkenningCommand();
        Command = configure(Command);

        return this;
    }

    public WijzigErkenningContext<TScenario> WithInitiator(string? ovoCode = null)
    {
        Metadata = Metadata with { Initiator = ovoCode ?? Fixture.Create<string>() };

        return this;
    }

    public async ValueTask Handle(
        WijzigErkenningCommand command,
        CommandMetadata? metadata = null,
        IOrganisatieBevoegdheidService? service = null)
        => await _handler.Handle(
            new CommandEnvelope<WijzigErkenningCommand>(command, metadata ?? Metadata),
            service ?? OrganisatieBevoegdheidService.Object);

    public async ValueTask<WijzigErkenningContext<TScenario>> WhenHandled()
    {
        await _handler.Handle(
            new CommandEnvelope<WijzigErkenningCommand>(Command, Metadata),
            OrganisatieBevoegdheidService.Object
        );

        return this;
    }

    public void ShouldHaveSaved(params IEvent[] events) => AggregateSessionMock.ShouldHaveSavedExact(events);

    public void ShouldHaveSavedErkenningWerdGewijzigd(ErkenningWerdGewijzigd gewijzigd, params IEvent[] precedingEvents)
    {
        var events = precedingEvents.ToList();
        events.Add(gewijzigd);

        if (gewijzigd.Status == ErkenningStatus.Actief.Value && !Scenario.GetVerenigingState().IsErkend)
            events.Add(new VerenigingWerdErkend());

        ShouldHaveSaved(events.ToArray());
    }

    private ErkenningWerdGeregistreerd OrigineleErkenning() =>
        Scenario
           .Events()
           .OfType<ErkenningWerdGeregistreerd>()
           .Single(e => e.ErkenningId == Command.Erkenning.ErkenningId);

    public string GetInitiatorOvoCode() => OrigineleErkenning().GeregistreerdDoor.OvoCode;

    private WijzigErkenningCommand CreateWijzigErkenningCommand()
        => Fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = Scenario.VCode,
            Erkenning = TeWijzigenErkenning.Create(
                _erkenningId,
                startDatum: NullOrEmpty<DateOnly>.Null,
                eindDatum: NullOrEmpty<DateOnly>.Null,
                hernieuwingsDatum: NullOrEmpty<DateOnly>.Null,
                hernieuwingsUrl: null,
                redenVanWijziging: Fixture.Create<string>()
            ),
        };



    public ErkenningWerdGewijzigd ExpectedErkenningWerdGewijzigd(
        DateOnly? startdatum = null,
        DateOnly? einddatum = null,
        DateOnly? hernieuwingsdatum = null,
        string? hernieuwingsUrl = null,
        string? status = null,
        string? redenVanWijziging = null
    )
    {
        var origineel = OrigineleErkenning();

        var effectiveStart = startdatum ?? origineel.Startdatum;
        var effectiveEind = einddatum ?? origineel.Einddatum;
        var effectiveHernieuwingsdatum = hernieuwingsdatum ?? origineel.Hernieuwingsdatum;
        var effectiveUrl = hernieuwingsUrl ?? origineel.HernieuwingsUrl;

        var effectiveStatus =
            status
         ?? ErkenningStatus
           .Bepaal(ErkenningsPeriode.Create(effectiveStart, effectiveEind), DateOnly.FromDateTime(DateTime.Today))
           .Value;

        return new ErkenningWerdGewijzigd(
            Command.Erkenning.ErkenningId,
            effectiveStart,
            effectiveEind,
            effectiveHernieuwingsdatum,
            effectiveUrl,
            effectiveStatus,
            redenVanWijziging ?? Command.Erkenning.RedenVanWijziging
        );
    }
}
