namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Common.Scenarios.CommandHandling;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;

public class ErkenningScenarioBuilder
{
    private readonly IFixture _fixture;
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario VzerWerdGeregistreerdScenario = new();
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario VmrWerdGeregistreerdScenario = new();
    private DateOnly _today = DateOnly.FromDateTime(DateTime.Today);
    private IList<IEvent> Events = new List<IEvent>();
    public int ErkenningId { get; private set; }
    public CommandhandlerScenarioBase Vzer { get; private set; } = null!;
    public CommandhandlerScenarioBase Vmr { get; private set; } = null!;

    public ErkenningScenarioBuilder(IFixture fixture)
    {
        _fixture = fixture;
        ErkenningId = fixture.Create<int>();
    }

    public ErkenningScenarioBuilder WithActieveErkenning()
    {
        var erkenningWerdGeregistreerd = _fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            ErkenningId = ErkenningId,
            Startdatum = _today.AddDays(-10),
            Einddatum = _today.AddDays(10),
            Hernieuwingsdatum = _today.AddDays(5),
            Status = ErkenningStatus.Actief.Value,
        };

        Events.Add(erkenningWerdGeregistreerd);

        return this;
    }

    public ErkenningScenarioBuilder WithVerlopenErkenning()
    {
        var erkenningWerdVerlopen = _fixture.Create<ErkenningWerdVerlopen>() with { ErkenningId = ErkenningId };

        Events.Add(erkenningWerdVerlopen);

        return this;
    }

    public ErkenningScenarioBuilder WithInAanvraagErkenning()
    {
        var startdatum = _today.AddDays(_fixture.Create<int>());

        var erkenningWerdGeregistreerd = _fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            ErkenningId = ErkenningId,
            Startdatum = startdatum,
            Hernieuwingsdatum = startdatum.AddDays(5),
            Einddatum = startdatum.AddDays(10),
            Status = ErkenningStatus.InAanvraag.Value,
        };

        Events.Add(erkenningWerdGeregistreerd);

        return this;
    }

    public ErkenningScenarioBuilder WithVerenigingWerdErkend()
    {
        Events.Add(new VerenigingWerdErkend());

        return this;
    }

    public ErkenningScenarioBuilder WithErkenningWerdGeschorst()
    {
        Events.Add(new ErkenningWerdGeschorst(ErkenningId, "Reden van schorsing"));

        return this;
    }

    public ErkenningScenarioBuilder WithVerenigingWerdNietLangerErkend()
    {
        Events.Add(new VerenigingWerdNietLangerErkend());

        return this;
    }

    public ErkenningScenarioBuilder WithTeVerlopenErkenning()
    {
        var einddatum = _today.AddDays(-_fixture.Create<int>());
        var hernieuwingsdatum = einddatum.AddDays(-_fixture.Create<int>());
        var startdatum = hernieuwingsdatum.AddDays(-_fixture.Create<int>());

        Events.Add(
            _fixture.Create<ErkenningWerdGeregistreerd>() with
            {
                ErkenningId = ErkenningId,
                Startdatum = startdatum,
                Hernieuwingsdatum = hernieuwingsdatum,
                Einddatum = einddatum,
                Status = ErkenningStatus.Actief.Value,
            }
        );

        return this;
    }

    public ErkenningScenarioBuilder WithSchorsingVanErkenningWerdOpgeheven()
    {
        Events.Add(new SchorsingVanErkenningWerdOpgeheven(ErkenningId, ErkenningStatus.Actief.Value));

        return this;
    }

    public ErkenningScenarioBuilder WithTeActiverenErkenning()
    {
        ErkenningId = _fixture.Create<int>();
        var startdatum = _today.AddDays(-_fixture.Create<int>());
        var einddatum = _today.AddDays(_fixture.Create<int>());
        var hernieuwingsdatum = einddatum.AddDays(_fixture.Create<int>());

        var erkenningWerdGeregistreerd = _fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            ErkenningId = ErkenningId,
            Startdatum = startdatum,
            Einddatum = einddatum,
            Hernieuwingsdatum = hernieuwingsdatum,
            Status = ErkenningStatus.InAanvraag.Value,
        };

        Events.Add(erkenningWerdGeregistreerd);

        return this;
    }

    public ErkenningScenarioBuilder WithErkenningOpvolgersWerdenToegevoegdAlsBeheerder()
    {
        Events.Add(
            _fixture.Create<ErkenningOpvolgersWerdenToegevoegdAlsBeheerder>() with
            {
                ErkenningId = ErkenningId,
            }
        );

        return this;
    }

    public ErkenningScenarioBuilder WithGewijzigdNaarInAanvraag()
    {
        var startdatum = _today.AddDays(_fixture.Create<int>());

        Events.Add(
            new ErkenningWerdGewijzigd(
                ErkenningId,
                startdatum,
                startdatum.AddDays(10),
                startdatum.AddDays(5),
                null,
                null,
                "test"
            )
        );

        return this;
    }

    public ErkenningScenarioBuilder Build()
    {
        BuildForVzer();
        BuildForVmr();

        return this;
    }

    public void BuildForVzer()
    {
        VzerWerdGeregistreerdScenario.additionalEvents.AddRange(Events);

        Vzer = VzerWerdGeregistreerdScenario;
    }

    public void BuildForVmr()
    {
        VmrWerdGeregistreerdScenario.additionalEvents.AddRange(Events);

        Vmr = VmrWerdGeregistreerdScenario;
    }
}
