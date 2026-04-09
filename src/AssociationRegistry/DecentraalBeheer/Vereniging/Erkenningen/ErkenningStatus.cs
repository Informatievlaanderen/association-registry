namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;


public class ErkenningStatus
{
    public string Value { get; }

    private ErkenningStatus(string status)
    {
        Value = status;
    }

    public static ErkenningStatus Create(DateOnly startDatum, DateOnly eindDatum)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);

        if (startDatum > now)
        {
            return new ErkenningStatus(InAanvraag);
        }

        if (eindDatum < now)
        {
            return new ErkenningStatus(Verlopen);
        }

        return new ErkenningStatus(Actief);
    }

    private const string Actief = "Actief";
    private const string Verlopen = "Verlopen";
    private const string InAanvraag = "InAanvraag";
}
