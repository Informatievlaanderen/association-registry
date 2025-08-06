namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Framework;
using Exceptions;

public record Insz
{
    public string Value { get; }

    private Insz(string value)
    {
        Value = value;
    }

    public static Insz Create(string insz)
    {
        var sanitezedInsz = Sanitize(insz);
        Validate(sanitezedInsz);

        return new Insz(sanitezedInsz);
    }

    internal static Insz Hydrate(string insz)
        => new(insz);

    public override string ToString()
        => Value;

    public static implicit operator string(Insz insz)
        => insz.Value;

    private static string Sanitize(string insz)
        => insz.Replace(oldValue: ".", string.Empty).Replace(oldValue: "-", string.Empty);

    private static void Validate(string sanitezedInsz)
    {
        Throw<InszLengteIsOngeldig>.If(sanitezedInsz.Length != 11);
        Throw<InszBevatOngeldigeTekens>.IfNot(ulong.TryParse(sanitezedInsz, out _));
        Throw<InszMod97IsOngeldig>.IfNot(Modulo97Correct(sanitezedInsz) ^ Modulo97Correct($"2{sanitezedInsz}"));
    }

    private static bool Modulo97Correct(string value)
    {
        var baseNumber = long.Parse(value[..^2]);
        var remainder = long.Parse(value[^2..]);

        var modulo97 = 97 - baseNumber % 97;

        return modulo97 == remainder;
    }
}
