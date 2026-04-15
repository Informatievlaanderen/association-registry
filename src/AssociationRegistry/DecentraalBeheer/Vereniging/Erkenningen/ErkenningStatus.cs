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

        if (startDatum is null && eindDatum is null || startDatum < today && eindDatum is null || startDatum == today && eindDatum is null)
        {
            return Actief;
        }

        if (startDatum > today && eindDatum is null)
        {
            return InAanvraag;
        }

        if (startDatum is null && eindDatum < today || startDatum < today && eindDatum < today)
        {
            return Verlopen;
        }

        if (startDatum is null && eindDatum == today || startDatum < today && eindDatum == today || startDatum == today && eindDatum == today || startDatum is null && eindDatum > today || startDatum < today && eindDatum > today || startDatum == today && eindDatum > today)
        {
            return Actief;
        }

        if (startDatum > today && eindDatum > today)
        {
            return InAanvraag;
        }

        return string.Empty;
        // TODO error?
    }

    private const string Actief = "Actief";
    private const string Verlopen = "Verlopen";
    private const string InAanvraag = "InAanvraag";
}
