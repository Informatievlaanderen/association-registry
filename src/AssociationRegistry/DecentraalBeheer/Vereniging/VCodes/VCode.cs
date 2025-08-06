namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;

public class VCode : ValueObject<VCode>
{
    public const int StartingVCode = 1001;
    private readonly int _code;

    private VCode(int code)
    {
        _code = code;
    }

    public string Value
        => $"V{_code:0000000}";

    public static VCode Create(string vCode)
    {
        ValidateVCode(vCode);

        return Create(int.Parse(vCode[1..]));
    }

    public static void ValidateVCode(string vCode)
    {
        Throw<VCodeFormaatIsOngeldig>.IfNot(VCodeHasLengthEight(vCode));
        Throw<VCodeFormaatIsOngeldig>.IfNot(VCodeStartsWith_V(vCode));
        Throw<VCodeFormaatIsOngeldig>.IfNot(VCodeAfterV_IsNumeric(vCode));
    }

    public static VCode Create(int vCode)
    {
        Throw<VCodeValtBuitenToegelatenWaardes>.If(VCodeLessThanStartingVCode(vCode));

        return new VCode(vCode);
    }

    internal static VCode Hydrate(string vCode)
        => new(int.Parse(vCode[1..]));

    private static bool VCodeHasLengthEight(string vCode)
        => vCode.Length == 8;

    protected override IEnumerable<object> Reflect()
    {
        yield return _code;
    }

    public override string ToString()
        => Value;

    private static bool VCodeAfterV_IsNumeric(string vCode)
        => int.TryParse(vCode[1..], out _);

    private static bool VCodeStartsWith_V(string vCode)
        => vCode.ToUpper().StartsWith(value: 'V');

    private static bool VCodeLessThanStartingVCode(int vCode)
        => vCode < StartingVCode;
}
