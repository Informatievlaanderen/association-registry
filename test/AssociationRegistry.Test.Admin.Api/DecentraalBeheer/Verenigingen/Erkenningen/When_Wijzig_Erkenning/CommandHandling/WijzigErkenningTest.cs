namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling;

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

public class WijzigErkenningTest<TScenario>
    where TScenario : CommandhandlerScenarioBase
{
    private readonly Fixture _fixture;
    private readonly WijzigErkenningCommandHandler _handler;
    private readonly Func<TScenario, int> _erkenningIdSelector;

    private WijzigErkenningCommand _command;
    private CommandMetadata _metadata;

    public TScenario Scenario { get; }
    public AggregateSessionMock AggregateSessionMock { get; }
    public IOrganisatieBevoegdheidServiceMockStub OrganisatieBevoegdheidService { get; }

    private WijzigErkenningTest(TScenario scenario, Func<TScenario, int> erkenningIdSelector)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        Scenario = scenario;
        _erkenningIdSelector = erkenningIdSelector;
        AggregateSessionMock = new AggregateSessionMock(scenario.GetVerenigingState());
        _handler = new WijzigErkenningCommandHandler(AggregateSessionMock);
        OrganisatieBevoegdheidService = new IOrganisatieBevoegdheidServiceMockStub();

        var erkenningId = erkenningIdSelector(scenario);

        _command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = scenario.VCode,
            Erkenning = TeWijzigenErkenning.Create(
                erkenningId,
                startDatum: NullOrEmpty<DateOnly>.Null,
                eindDatum: NullOrEmpty<DateOnly>.Null,
                hernieuwingsDatum: NullOrEmpty<DateOnly>.Null,
                hernieuwingsUrl: null,
                redenVanWijziging: _fixture.Create<string>()
            ),
        };

        _metadata = _fixture.Create<CommandMetadata>();
    }

    public static WijzigErkenningTest<TScenario> Given(TScenario scenario, Func<TScenario, int> erkenningIdSelector) =>
        new(scenario, erkenningIdSelector);

    public WijzigErkenningTest<TScenario> WithCommand(Func<WijzigErkenningCommand, WijzigErkenningCommand> configure)
    {
        _command = configure(_command);
        return this;
    }

    public WijzigErkenningTest<TScenario> WithInitiator(string? ovoCode = null)
    {
        _metadata = _metadata with { Initiator = ovoCode ?? _fixture.Create<string>() };
        return this;
    }

    public async ValueTask<WijzigErkenningTest<TScenario>> WhenHandled()
    {
        await _handler.Handle(
            new CommandEnvelope<WijzigErkenningCommand>(_command, _metadata),
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
            .Single(e => e.ErkenningId == _command.Erkenning.ErkenningId);

    public string GetInitiatorOvoCode() => OrigineleErkenning().GeregistreerdDoor.OvoCode;

    public WijzigErkenningTest<TScenario> WithDefaultErkenningModification()
    {
        var origineleErkenning = OrigineleErkenning();
        // Modify the HernieuwingsUrl to a predictable value to ensure a change
        _command = _command with
        {
            Erkenning = _command.Erkenning with { HernieuwingsUrl = "https://example.org/renewal" },
        };
        return this;
    }

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
            _command.Erkenning.ErkenningId,
            effectiveStart,
            effectiveEind,
            effectiveHernieuwingsdatum,
            effectiveUrl,
            effectiveStatus,
            redenVanWijziging ?? _command.Erkenning.RedenVanWijziging
        );
    }
}
