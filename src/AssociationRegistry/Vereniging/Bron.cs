namespace AssociationRegistry.Vereniging;

public class Bron
{
    public static readonly Bron Kbo = new("KBO", "Kruispuntbank van de Ondernemingen");

    public static readonly Bron[] All = { Kbo };

    public Bron(string waarde, string betekenis)
    {
        Waarde = waarde;
        Betekenis = betekenis;
    }

    public string Waarde { get; }
    public string Betekenis { get; }

    public static Bron Parse(string waarde)
        => All.Single(t => t.Waarde == waarde);

    public static implicit operator string(Bron bron)
        => bron.Waarde;

    public static implicit operator Bron(string bronString)
        => Parse(bronString);
}
