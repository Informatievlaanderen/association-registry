namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using Primitives;

public sealed record ErkenningCorrectie
{
    private ErkenningCorrectie(
        int erkenningErkenningId,
        ErkenningsPeriode erkenningsperiode,
        Hernieuwingsdatum hernieuwingsdatum,
        HernieuwingsUrl hernieuwingsUrl)
    {
        ErkenningId = erkenningErkenningId;
        ErkenningsPeriode = erkenningsperiode;
        Hernieuwingsdatum = hernieuwingsdatum;
        HernieuwingsUrl = hernieuwingsUrl;
    }

    public int ErkenningId { get; set; }
    public ErkenningsPeriode ErkenningsPeriode { get; set; }
    public Hernieuwingsdatum Hernieuwingsdatum { get; set; }
    public HernieuwingsUrl HernieuwingsUrl { get; set; }

    public static ErkenningCorrectie Create(
        TeCorrigerenErkenning teCorrigerenErkenning,
        Erkenning erkenning)
    {
        var startdatum =
            DetermineTeCorrigerenDatum(teCorrigerenErkenning.StartDatum, erkenning.ErkenningsPeriode.Startdatum);
        var einddatum =
            DetermineTeCorrigerenDatum(teCorrigerenErkenning.EindDatum, erkenning.ErkenningsPeriode.Einddatum);
        var erkenningsperiode = ErkenningsPeriode.Create(startdatum, einddatum);

        var hernieuwingsdatumDate =
            DetermineTeCorrigerenDatum(teCorrigerenErkenning.Hernieuwingsdatum, erkenning.Hernieuwingsdatum.Value);
        var hernieuwingsdatum = Hernieuwingsdatum.Create(hernieuwingsdatumDate, erkenningsperiode);

        var teCorrigerenHernieuwingsUrl = DetermineTeCorrigerenHernieuwingsUrl(teCorrigerenErkenning, erkenning);
        var hernieuwingsUrl = HernieuwingsUrl.Create(teCorrigerenHernieuwingsUrl);

        return new ErkenningCorrectie(erkenning.ErkenningId, erkenningsperiode, hernieuwingsdatum, hernieuwingsUrl);
    }

    private static DateOnly? DetermineTeCorrigerenDatum(
        NullOrEmpty<DateOnly> commandValue,
        DateOnly? stateValue)
        => commandValue.IsNull ? stateValue :
            commandValue.IsEmpty ? null : commandValue.Value;

    private static string? DetermineTeCorrigerenHernieuwingsUrl(
        TeCorrigerenErkenning teCorrigerenErkenning,
        Erkenning erkenning)
        => teCorrigerenErkenning.HernieuwingsUrl is not null
            ? teCorrigerenErkenning.HernieuwingsUrl
            : erkenning.HernieuwingsUrl.Value;

    public bool HeeftWijzigingen(Erkenning erkenning)
        => ErkenningsPeriode != erkenning.ErkenningsPeriode
        || Hernieuwingsdatum != erkenning.Hernieuwingsdatum
        || HernieuwingsUrl != erkenning.HernieuwingsUrl;
}
