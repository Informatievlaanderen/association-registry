namespace AssociationRegistry.VCodes;

using System.Collections.Generic;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;
using Framework;

public class VCode : ValueObject<VCode>
{
    private readonly int _code;

    public const int StartingVCode = 1001;

    public static VCode Create(string vCode)
    {
        Throw<InvalidVCodeFormat>.IfNot(VCodeHasLengthEight(vCode));
        Throw<InvalidVCodeFormat>.IfNot(VCodeStartsWith_V(vCode));
        Throw<InvalidVCodeFormat>.IfNot(VCodeAfterV_IsNumeric(vCode));

        return Create(int.Parse(vCode[1..]));
    }

    private static bool VCodeHasLengthEight(string vCode)
        => vCode.Length == 8;

    public static VCode Create(int vCode)
    {
        Throw<OutOfRangeVCode>.If(VCodeLessThanStartingVCode(vCode));
        return new VCode(vCode);
    }

    private VCode(int code)
    {
        _code = code;
    }

    public string Value
        => $"V{_code:0000000}";

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

    private static bool VCodeLessThanStartingVCode(int vCode)
        => vCode < StartingVCode;
}
