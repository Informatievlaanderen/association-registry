namespace AssociationRegistry.Vereniging;

public class Verenigingstype
{
    public static readonly Verenigingstype FeitelijkeVereniging = new("FV", "Feitelijke vereniging");
    public static readonly Verenigingstype VZW = new("VZW", "Vereniging zonder winstoogmerk");
    public static readonly Verenigingstype IVZW = new("IVZW", "Internationale vereniging zonder winstoogmerk");
    public static readonly Verenigingstype PrivateStichting = new("PS", "Private stichting");
    public static readonly Verenigingstype StichtingVanOpenbaarNut = new("SVON", "Stichting van openbaar nut");
    public static readonly Verenigingstype Afdeling = new("AFD", "Afdeling");

    public static readonly Verenigingstype[] All = { FeitelijkeVereniging, VZW, Afdeling };

    public Verenigingstype(string code, string beschrijving)
    {
        Code = code;
        Beschrijving = beschrijving;
    }

    public string Code { get; }
    public string Beschrijving { get; }

    public static Verenigingstype Parse(string code)
        => All.Single(t => t.Code == code);
}
