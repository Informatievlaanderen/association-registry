namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;

public class ErkenningScenarioBuilder
{
    private readonly IFixture _fixture;
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario VzerWerdGeregistreerdScenario = new();
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario VmrWerdGeregistreerdScenario = new();
    private DateOnly _today = DateOnly.FromDateTime(DateTime.Today);
    private int _erkenningId;
    private IList<IEvent> Events = new List<IEvent>();

    public ErkenningScenarioBuilder(IFixture fixture)
    {
        _fixture = fixture;
        _erkenningId = fixture.Create<int>();
    }

    public ErkenningScenarioBuilder WithActieveErkenning()
    {
        var erkenningWerdGeregistreerd = _fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            ErkenningId = _erkenningId,
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
        var erkenningWerdVerlopen = _fixture.Create<ErkenningWerdVerlopen>() with { ErkenningId = _erkenningId };

        Events.Add(erkenningWerdVerlopen);

        return this;
    }

    public ErkenningScenarioBuilder WithInAanvraagErkenning()
    {
        var startdatum = _today.AddDays(_fixture.Create<int>());

        var erkenningWerdGeregistreerd = _fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            ErkenningId = _erkenningId,
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
        Events.Add(new ErkenningWerdGeschorst(_erkenningId, "Reden van schorsing"));

        return this;
    }

    public ErkenningScenarioBuilder WithVerenigingWerdNietLangerErkend()
    {
        Events.Add(new VerenigingWerdNietLangerErkend());

        return this;
    }

    public ErkenningScenarioBuilder WithTeActiverenErkenning()
    {
        _erkenningId = _fixture.Create<int>();
        var startdatum = _today.AddDays(-_fixture.Create<int>());
        var einddatum = _today.AddDays(_fixture.Create<int>());
        var hernieuwingsdatum = einddatum.AddDays(_fixture.Create<int>());

        var erkenningWerdGeregistreerd = _fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            ErkenningId = _erkenningId,
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
                ErkenningId = _erkenningId,
            }
        );

        return this;
    }

    public ErkenningScenarioBuilder WithGewijzigdNaarInAanvraag()
    {
        var startdatum = _today.AddDays(_fixture.Create<int>());

        Events.Add(
            new ErkenningWerdGewijzigd(
                _erkenningId,
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

    public (int, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario) BuildForVzer()
    {
        VzerWerdGeregistreerdScenario.additionalEvents.AddRange(Events);

        return (_erkenningId, VzerWerdGeregistreerdScenario);
    }

    public (int, VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario) BuildForVmr()
    {
        VmrWerdGeregistreerdScenario.additionalEvents.AddRange(Events);

        return (_erkenningId, VmrWerdGeregistreerdScenario);
    }
}
