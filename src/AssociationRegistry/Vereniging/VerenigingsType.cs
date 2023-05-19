namespace AssociationRegistry.Vereniging;

public class VerenigingsType
{
    public static readonly VerenigingsType FeitelijkeVereniging = new("FV", "Feitelijke vereniging");
    public static readonly VerenigingsType VerenigingMetRechtspersoonlijkheid = new("VZW", "Vereniging zonder winstoogmerk");

    public static readonly VerenigingsType[] All = { FeitelijkeVereniging, VerenigingMetRechtspersoonlijkheid };

    public VerenigingsType(string code, string beschrijving)
    {
        Code = code;
        Beschrijving = beschrijving;
    }

    public string Code { get; }
    public string Beschrijving { get; }

    public static VerenigingsType Parse(string code)
        => All.Single(t => t.Code == code);
}
