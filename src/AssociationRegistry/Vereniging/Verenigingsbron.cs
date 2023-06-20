namespace AssociationRegistry.Vereniging;

public class Verenigingsbron
{
    public static readonly Verenigingsbron Kbo = new("KBO", "Kruispuntbank van de Ondernemingen");

    public static readonly Verenigingsbron[] All = { Kbo };

    public Verenigingsbron(string waarde, string betekenis)
    {
        Waarde = waarde;
        Betekenis = betekenis;
    }

    public string Waarde { get; }
    public string Betekenis { get; }

    public static Verenigingsbron Parse(string waarde)
        => All.Single(t => t.Waarde == waarde);

    public static implicit operator string(Verenigingsbron verenigingsbron)
        => verenigingsbron.Waarde;

    public static implicit operator Verenigingsbron(string bronString)
        => Parse(bronString);
}
