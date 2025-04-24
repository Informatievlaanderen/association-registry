namespace AssociationRegistry.Vereniging;

public class Verenigingssubtype : IVerenigingssubtype
{
    public static class Codes
    {
        public const string FV = "FV";
        public const string Sub = "SUB";
        public const string NB = "NB";

    }
    public static readonly Verenigingssubtype FeitelijkeVereniging = new(code: Codes.FV, naam: "Feitelijke vereniging");
    public static readonly Verenigingssubtype Subvereniging = new(code: Codes.Sub, naam: "Subvereniging");
    public static readonly Verenigingssubtype NietBepaald = new(code: Codes.NB, naam: "Niet bepaald");

    public static readonly Verenigingssubtype[] All =
    {
        FeitelijkeVereniging,
        Subvereniging,
        NietBepaald,
    };

    public static readonly Verenigingssubtype Default = new (string.Empty, string.Empty);

    public Verenigingssubtype(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    public string Code { get; init; }
    public string Naam { get; init; }

    public static Verenigingssubtype Parse(string code)
        => All.Single(t => t.Code == code);

    public static string GetNameOrDefaultOrNull(string code)
        => code switch
        {
            null => throw new ArgumentNullException(nameof(code)),
            "" => Default.Naam,
            _ => All.Single(t => t.Code == code).Naam,
        };

    public static bool IsValidSubtypeCode(string code)
        => All.Any(t => t.Code == code);

    public static bool IsGeenSubVereniging(string code)
        => code != Subvereniging.Code;

    public bool IsFeitelijkeVereniging
        => Code == FeitelijkeVereniging.Code;

    public bool IsNietBepaald
        => Code == NietBepaald.Code;

    public bool IsSubVereniging
        => Code == Subvereniging.Code;

    // private record DefaultVerenigingssubtype : IVerenigingssubtype
    // {
    //     public DefaultVerenigingssubtype()
    //     {
    //         Code = string.Empty;
    //         Naam = string.Empty;
    //     }
    //     public string Code { get; init; }
    //     public string Naam { get; init; }
    // }
}

public static class VerenigingssubtypeExtensions
{
    public static TDestination Convert<TDestination>(this IVerenigingssubtype subtype) where TDestination : IVerenigingssubtype, new()
        => new()
        {
            Code = subtype.Code,
            Naam = subtype.Naam,
        };
}

public interface IVerenigingssubtype
{
    string Code { get; init; }
    string Naam { get; init; }
}
