namespace AssociationRegistry.Vereniging;

public class Subtype : ISubtype
{
    public static readonly Subtype FeitelijkeVereniging = new(code: "FV", naam: "Feitelijke vereniging");
    public static readonly Subtype SubVereniging = new(code: "SUB", naam: "Subvereniging");
    public static readonly Subtype NogNietBepaald = new(code: "NNB", naam: "Nog niet bepaald");

    public static readonly Subtype[] All =
    {
        FeitelijkeVereniging,
        SubVereniging,
        NogNietBepaald,
    };

    public Subtype(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    public string Code { get; init; }
    public string Naam { get; init; }

    public static Subtype Parse(string code)
        => All.Single(t => t.Code == code);

    public static bool IsGeenSubVereniging(string code)
        => code != SubVereniging.Code;
}

public interface ISubtype
{
    string Code { get; init; }
    string Naam { get; init; }
}
