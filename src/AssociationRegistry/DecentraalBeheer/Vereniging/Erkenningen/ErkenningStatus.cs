namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public class ErkenningStatus
{
    public string Value { get; }

    private ErkenningStatus(string status)
    {
        Value = status;
    }

    public static string Bepaal(ErkenningsPeriode erkenningsPeriode, DateOnly today)
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

    public const string Actief = "Actief";
    public const string Verlopen = "Verlopen";
    public const string InAanvraag = "InAanvraag";
    public const string Geschorst = "Geschorst";
}
