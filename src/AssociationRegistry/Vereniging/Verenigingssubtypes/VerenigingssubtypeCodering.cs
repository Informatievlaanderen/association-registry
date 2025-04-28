namespace AssociationRegistry.Vereniging;

public record VerenigingssubtypeCodering(string Code, string Naam): IVerenigingssubtypeCode
{
    public static VerenigingssubtypeCodering FeitelijkeVereniging = new VerenigingssubtypeCodering(Fv, "Feitelijke vereniging");
    public static VerenigingssubtypeCodering SubverenigingVan = new VerenigingssubtypeCodering(Sub, "Subvereniging");
    public static VerenigingssubtypeCodering NietBepaald = new VerenigingssubtypeCodering(Nb, "Niet bepaald");
    public static VerenigingssubtypeCodering Default = new VerenigingssubtypeCodering(DefaultCode, "");

    public static VerenigingssubtypeCodering Parse(string code)
        => All.Single(t => t.Code == code);

    public static readonly VerenigingssubtypeCodering[] All =
    {
        FeitelijkeVereniging,
        SubverenigingVan,
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
        => code != SubverenigingVan.Code;

    public bool IsFeitelijkeVereniging
        => Code == FeitelijkeVereniging.Code;

    public bool IsNietBepaald
        => Code == NietBepaald.Code;

    public bool IsSubVereniging
        => Code == SubverenigingVan.Code;

    private const string Fv = "FV";
    private const string Sub = "SUB";
    private const string Nb = "NB";
    private const string DefaultCode = "";
}