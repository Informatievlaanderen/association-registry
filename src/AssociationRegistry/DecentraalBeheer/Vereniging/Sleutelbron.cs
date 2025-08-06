namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public class Sleutelbron
{
    public static readonly Sleutelbron KBO = new(waarde: "KBO", betekenis: "Kruispuntbank van de Ondernemingen");
    public static readonly Sleutelbron VR = new(waarde: "Vcode", betekenis: "Vcode");
    public static readonly Sleutelbron[] All = { KBO , VR};

    public Sleutelbron(string waarde, string betekenis)
    {
        Waarde = waarde;
        Betekenis = betekenis;
    }

    public string Waarde { get; }
    public string Betekenis { get; }

    public static Sleutelbron Parse(string waarde)
        => All.Single(t => t.Waarde == waarde);

    public static implicit operator string(Sleutelbron sleutelbron)
        => sleutelbron.Waarde;

    public static implicit operator Sleutelbron(string bronString)
        => Parse(bronString);
}
