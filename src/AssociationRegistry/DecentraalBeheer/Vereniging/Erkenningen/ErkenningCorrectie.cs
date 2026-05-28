namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using Primitives;

public sealed record ErkenningCorrectie
{
    private ErkenningCorrectie(
        int erkenningErkenningId,
        ErkenningsPeriode erkenningsperiode,
        Hernieuwingsdatum hernieuwingsdatum,
        HernieuwingsUrl hernieuwingsUrl,
        ErkenningStatus status
    )
    {
        ErkenningId = erkenningErkenningId;
        ErkenningsPeriode = erkenningsperiode;
        Hernieuwingsdatum = hernieuwingsdatum;
        HernieuwingsUrl = hernieuwingsUrl;
        Status = status;
    }

    public int ErkenningId { get; set; }
    public ErkenningsPeriode ErkenningsPeriode { get; set; }
    public Hernieuwingsdatum Hernieuwingsdatum { get; set; }
    public HernieuwingsUrl HernieuwingsUrl { get; set; }
    public ErkenningStatus Status { get; set; }

    public static ErkenningCorrectie Create(TeCorrigerenErkenning teCorrigerenErkenning, Erkenning erkenning)
    {
        var startdatum = DetermineTeCorrigerenDatum(
            teCorrigerenErkenning.StartDatum,
            erkenning.ErkenningsPeriode.Startdatum
        );
        var einddatum = DetermineTeCorrigerenDatum(
            teCorrigerenErkenning.EindDatum,
            erkenning.ErkenningsPeriode.Einddatum
        );
        var erkenningsperiode = ErkenningsPeriode.Create(startdatum, einddatum);

        var hernieuwingsdatumDate = DetermineTeCorrigerenDatum(
            teCorrigerenErkenning.Hernieuwingsdatum,
            erkenning.Hernieuwingsdatum.Value
        );
        var hernieuwingsdatum = Hernieuwingsdatum.Create(hernieuwingsdatumDate, erkenningsperiode);

        var hernieuwingsUrl = DetermineTeCorrigerenHernieuwingsUrl(
            teCorrigerenErkenning.HernieuwingsUrl,
            erkenning.HernieuwingsUrl
        );

        var status = ErkenningStatus.BepaalVoorCorrectie(
            erkenning.Status,
            erkenningsperiode,
            DateOnly.FromDateTime(DateTime.Today)
        );

        return new ErkenningCorrectie(
            erkenning.ErkenningId,
            erkenningsperiode,
            hernieuwingsdatum,
            hernieuwingsUrl,
            status
        );
    }
    public static ErkenningCorrectie Create(TeWijzigenErkenning teWijzigenErkenning, Erkenning erkenning)
    {
        var startdatum = DetermineTeCorrigerenDatum(
            teWijzigenErkenning.StartDatum,
            erkenning.ErkenningsPeriode.Startdatum
        );
        var einddatum = DetermineTeCorrigerenDatum(
            teWijzigenErkenning.EindDatum,
            erkenning.ErkenningsPeriode.Einddatum
        );
        var erkenningsperiode = ErkenningsPeriode.Create(startdatum, einddatum);

        var hernieuwingsdatumDate = DetermineTeCorrigerenDatum(
            teWijzigenErkenning.Hernieuwingsdatum,
            erkenning.Hernieuwingsdatum.Value
        );
        var hernieuwingsdatum = Hernieuwingsdatum.Create(hernieuwingsdatumDate, erkenningsperiode);

        var hernieuwingsUrl = DetermineTeCorrigerenHernieuwingsUrl(
            teWijzigenErkenning.HernieuwingsUrl,
            erkenning.HernieuwingsUrl
        );

        var status = ErkenningStatus.BepaalVoorCorrectie(
            erkenning.Status,
            erkenningsperiode,
            DateOnly.FromDateTime(DateTime.Today)
        );

        return new ErkenningCorrectie(
            erkenning.ErkenningId,
            erkenningsperiode,
            hernieuwingsdatum,
            hernieuwingsUrl,
            status
        );
    }

    private static DateOnly? DetermineTeCorrigerenDatum(NullOrEmpty<DateOnly> commandValue, DateOnly? stateValue) =>
        commandValue.IsNull ? stateValue
        : commandValue.IsEmpty ? null
        : commandValue.Value;

    private static HernieuwingsUrl DetermineTeCorrigerenHernieuwingsUrl(
        string? teCorrigerenHernieuwingsUrl,
        HernieuwingsUrl hernieuwingsUrl
    ) =>
        teCorrigerenHernieuwingsUrl is not null ? HernieuwingsUrl.Create(teCorrigerenHernieuwingsUrl) : hernieuwingsUrl;

    public bool HeeftWijzigingen(Erkenning erkenning) =>
        ErkenningsPeriode != erkenning.ErkenningsPeriode
        || Hernieuwingsdatum != erkenning.Hernieuwingsdatum
        || HernieuwingsUrl != erkenning.HernieuwingsUrl;
}
