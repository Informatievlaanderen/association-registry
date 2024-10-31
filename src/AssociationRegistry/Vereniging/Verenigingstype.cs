namespace AssociationRegistry.Vereniging;

public class Verenigingstype
{
    public static readonly Verenigingstype FeitelijkeVereniging = new(code: "FV", naam: "Feitelijke vereniging");
    public static readonly Verenigingstype VZW = new(code: "VZW", naam: "Vereniging zonder winstoogmerk");
    public static readonly Verenigingstype IVZW = new(code: "IVZW", naam: "Internationale vereniging zonder winstoogmerk");
    public static readonly Verenigingstype PrivateStichting = new(code: "PS", naam: "Private stichting");
    public static readonly Verenigingstype StichtingVanOpenbaarNut = new(code: "SVON", naam: "Stichting van openbaar nut");
    public static readonly Verenigingstype Subvereniging = new(code: "SUB", naam: "Subvereniging van een andere vereniging");

    public static readonly Verenigingstype[] All =
    {
        FeitelijkeVereniging,
        VZW,
        IVZW,
        PrivateStichting,
        StichtingVanOpenbaarNut,
        Subvereniging,
    };

    public Verenigingstype(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    public string Code { get; }
    public string Naam { get; }

    public static Verenigingstype Parse(string code)
        => All.Single(t => t.Code == code);
}
