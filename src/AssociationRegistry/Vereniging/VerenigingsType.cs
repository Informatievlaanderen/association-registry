namespace AssociationRegistry.Vereniging;

public class VerenigingsType
{
    public static readonly VerenigingsType FeitelijkeVereniging = new("FV", "Feitelijke Vereniging");

    public static readonly VerenigingsType[] All = { FeitelijkeVereniging };

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
