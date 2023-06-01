namespace AssociationRegistry.Vereniging;

public class Verenigingstype
{
    public static readonly Verenigingstype FeitelijkeVereniging = new("FV", "Feitelijke vereniging");
    public static readonly Verenigingstype VerenigingMetRechtspersoonlijkheid = new("VZW", "Vereniging zonder winstoogmerk");
    public static readonly Verenigingstype Afdeling = new("AFD", "Afdeling");

    public static readonly Verenigingstype[] All = { FeitelijkeVereniging, VerenigingMetRechtspersoonlijkheid, Afdeling };

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
