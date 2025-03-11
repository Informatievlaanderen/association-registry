namespace AssociationRegistry.Vereniging;

public class Verenigingssubtype : IVerenigingssubtype
{
    public static readonly Verenigingssubtype FeitelijkeVereniging = new(code: "FV", naam: "Feitelijke vereniging");
    public static readonly Verenigingssubtype SubVereniging = new(code: "SUB", naam: "Subvereniging");
    public static readonly Verenigingssubtype NietBepaald = new(code: "NB", naam: "Niet bepaald");

    public static readonly Verenigingssubtype[] All =
    {
        FeitelijkeVereniging,
        SubVereniging,
        NietBepaald,
    };

    public Verenigingssubtype(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    public string Code { get; init; }
    public string Naam { get; init; }

    public static Verenigingssubtype Parse(string code)
        => All.Single(t => t.Code == code);

    public static bool IsGeenSubVereniging(string code)
        => code != SubVereniging.Code;

    public bool IsFeitelijkeVereniging
        => Code == FeitelijkeVereniging.Code;

    public bool IsNietBepaald
        => Code == NietBepaald.Code;

    public bool IsSubVereniging
        => Code == SubVereniging.Code;
}

public interface IVerenigingssubtype
{
    string Code { get; init; }
    string Naam { get; init; }
}
