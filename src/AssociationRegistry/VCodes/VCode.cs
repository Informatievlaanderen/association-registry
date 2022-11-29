namespace AssociationRegistry.VCodes;

using System.Collections.Generic;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;

public class VCode : ValueObject<VCode>
{
    private readonly int _code;

    public static VCode Create(string vCode)
    {
        Throw<InvalidVCodeFormat>.IfNot(VCodeStartsWith_V(vCode));
        Throw<InvalidVCodeFormat>.IfNot(VCodeAfterV_IsNumeric(vCode));

        return new VCode(int.Parse(vCode[1..]));
    }

    public static VCode Create(int vCode) => new(vCode);

    public VCode(int code)
    {
        //Throw<OutOfRangeVCode>.IfNot(CodeIsBetween_0_And_999999(code));

        _code = code;
    }

    public string Value // original
        => $"V{_code:000000}";

    public string Value7
        => $"W{_code:0000000}";

    public string ValueNoPadding
        => $"X{_code:0}";

    public string ValueNoPaddingStartsWithK
        => $"Y{(_code + 1000):0}";

    protected override IEnumerable<object> Reflect()
    {
        yield return _code;
    }

    public override string ToString()
        => Value;

    private static bool VCodeAfterV_IsNumeric(string vCode)
        => int.TryParse(vCode[1..], out _);

    private static bool VCodeStartsWith_V(string vCode)
        => vCode.ToUpper().StartsWith('V');

    private static bool VCodeIsOfLength_7(string vCode)
        => vCode.Length == 7;

    private static bool CodeIsBetween_0_And_999999(int code)
        => code is < 1_000_000 and > 0;
}
