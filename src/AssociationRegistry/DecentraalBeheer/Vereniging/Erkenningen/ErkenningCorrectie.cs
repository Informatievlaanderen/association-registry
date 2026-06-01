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

    public static ErkenningCorrectie Create(TeWijzigenErkenning teWijzigenErkenning, Erkenning erkenning)
    {
        var startdatum = DetermineTeWijzigenDatum(
            teWijzigenErkenning.StartDatum,
            erkenning.ErkenningsPeriode.Startdatum
        );
        var einddatum = DetermineTeWijzigenDatum(
            teWijzigenErkenning.EindDatum,
            erkenning.ErkenningsPeriode.Einddatum
        );
        var erkenningsperiode = ErkenningsPeriode.Create(startdatum, einddatum);

        var hernieuwingsdatumDate = DetermineTeWijzigenDatum(
            teWijzigenErkenning.Hernieuwingsdatum,
            erkenning.Hernieuwingsdatum.Value
        );
        var hernieuwingsdatum = Hernieuwingsdatum.Create(hernieuwingsdatumDate, erkenningsperiode);

        var hernieuwingsUrl = DetermineTeWijzigenHernieuwingsUrl(
            teWijzigenErkenning.HernieuwingsUrl,
            erkenning.HernieuwingsUrl
        );

        var status = ErkenningStatus.BepaalVoorWijziging(
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

    private static DateOnly? DetermineTeWijzigenDatum(NullOrEmpty<DateOnly> commandValue, DateOnly? stateValue) =>
        commandValue.IsNull ? stateValue
        : commandValue.IsEmpty ? null
        : commandValue.Value;

    private static HernieuwingsUrl DetermineTeWijzigenHernieuwingsUrl(
        string? teCorrigerenHernieuwingsUrl,
        HernieuwingsUrl hernieuwingsUrl
    ) =>
        teCorrigerenHernieuwingsUrl is not null ? HernieuwingsUrl.Create(teCorrigerenHernieuwingsUrl) : hernieuwingsUrl;

    public bool HeeftWijzigingen(Erkenning erkenning) =>
        ErkenningsPeriode != erkenning.ErkenningsPeriode
        || Hernieuwingsdatum != erkenning.Hernieuwingsdatum
        || HernieuwingsUrl != erkenning.HernieuwingsUrl;
}
