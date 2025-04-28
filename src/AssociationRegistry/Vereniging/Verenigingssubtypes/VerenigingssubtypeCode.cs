namespace AssociationRegistry.Vereniging;

public record VerenigingssubtypeCode(string Code, string Naam): IVerenigingssubtypeCode
{

    private const string Fv = "FV";
    private const string Sub = "SUB";
    private const string Nb = "NB";
    private const string DefaultCode = "";

    public static VerenigingssubtypeCode FeitelijkeVereniging = new VerenigingssubtypeCode(Fv, "Feitelijke vereniging");
    public static VerenigingssubtypeCode Subvereniging = new VerenigingssubtypeCode(Sub, "Subvereniging");
    public static VerenigingssubtypeCode NietBepaald = new VerenigingssubtypeCode(Nb, "Niet bepaald");
    public static VerenigingssubtypeCode Default = new VerenigingssubtypeCode(DefaultCode, "");

    public static VerenigingssubtypeCode Parse(string code)
        => All.Single(t => t.Code == code);

    public static readonly VerenigingssubtypeCode[] All =
    {
        FeitelijkeVereniging,
        Subvereniging,
        NietBepaald,
    };

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
}
