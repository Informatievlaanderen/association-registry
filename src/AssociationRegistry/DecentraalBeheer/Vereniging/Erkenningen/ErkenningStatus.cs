namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;


public class ErkenningStatus
{
    public string Value { get; }

    private ErkenningStatus(string status)
    {
        Value = status;
    }

    public static string Calculate(DateOnly? startDatum, DateOnly? eindDatum)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        if (startDatum > today && eindDatum is null || startDatum > today && eindDatum > today)
            return InAanvraag;

        if (eindDatum < today)
            return Verlopen;

        return Actief;
    }

    public const string Actief = "Actief";
    public const string Verlopen = "Verlopen";
    public const string InAanvraag = "InAanvraag";
}
