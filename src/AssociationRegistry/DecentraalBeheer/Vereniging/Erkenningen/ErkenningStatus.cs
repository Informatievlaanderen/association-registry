namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public sealed record ErkenningStatus
{
    public static readonly ErkenningStatus InAanvraag = new(InAanvraagValue);
    public static readonly ErkenningStatus Verlopen = new(VerlopenValue);
    public static readonly ErkenningStatus Geschorst = new(GeschorstValue);
    public static readonly ErkenningStatus Actief = new(ActiefValue);

    public string Value { get; }

    private ErkenningStatus(string status)
    {
        Value = status;
    }

    public static ErkenningStatus Bepaal(ErkenningsPeriode erkenningsPeriode, DateOnly today)
    {
        if (
            erkenningsPeriode.Startdatum > today && erkenningsPeriode.Einddatum is null
            || erkenningsPeriode.Startdatum > today && erkenningsPeriode.Einddatum > today
        )
            return InAanvraag;

        if (erkenningsPeriode.Einddatum < today)
            return Verlopen;

        return Actief;
    }

    public static ErkenningStatus BepaalVoorWijziging(
        ErkenningStatus huidigeStatus,
        ErkenningsPeriode erkenningsPeriode,
        DateOnly today
    )
    {
        if (huidigeStatus == Geschorst)
            return Geschorst;

        return Bepaal(erkenningsPeriode, today);
    }

    public static ErkenningStatus Hydrate(string status) => new(status);

    private const string ActiefValue = "Actief";
    private const string VerlopenValue = "Verlopen";
    private const string InAanvraagValue = "InAanvraag";
    private const string GeschorstValue = "Geschorst";
}
