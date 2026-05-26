namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using Exceptions;

public record WijzigingsTypeErkenning
{
    public string Value { get; }

    private WijzigingsTypeErkenning(string value)
    {
        Value = value;
    }

    public static WijzigingsTypeErkenning Create(string type)
    {
        if (!IsValid(type))
            throw new HernieuwingsDatumMoetTussenStartEnEindDatumLiggen();

        return new WijzigingsTypeErkenning(type);
    }

    public static WijzigingsTypeErkenning Hydrate(string type) => new(type);

    private static bool IsValid(string type) =>
        IsCorrectTypeName(type);

    private static bool IsCorrectTypeName(string type)
        => type.Equals(CorrigeerValue, StringComparison.OrdinalIgnoreCase) ||
           type.Equals(VerlengValue, StringComparison.OrdinalIgnoreCase);

    public const string CorrigeerValue = "Corrigeer";
    public const string VerlengValue = "Verleng";

    public static readonly string[] All = [CorrigeerValue, VerlengValue];
}
