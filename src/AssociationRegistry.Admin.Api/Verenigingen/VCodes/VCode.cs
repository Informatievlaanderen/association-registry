namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes;

using System.Collections.Generic;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Exceptions;

public class VCode : ValueObject<VCode>
{
    private readonly int _code;

    public VCode(string vCode)
    {
        Throw<InvalidVCodeLength>.IfNot(VCodeIsOfLength_7(vCode));
        Throw<InvalidVCodeFormat>.IfNot(VCodeStartsWith_V(vCode));
        Throw<InvalidVCodeFormat>.IfNot(VCodeAfterV_IsNumeric(vCode));

        _code = int.Parse(vCode[1..]);
    }

    public VCode(int code)
    {
        Throw<OutOfRangeVCode>.IfNot(CodeIsBetween_0_And_999999(code));

        _code = code;
    }

    public string Value
        => $"V{_code:000000}";

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
