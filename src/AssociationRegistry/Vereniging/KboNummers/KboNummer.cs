﻿namespace AssociationRegistry.Vereniging;

using Framework;
using Exceptions;

public record KboNummer
{
    public static readonly KboNummer Leeg = new((string?)null);
    public string? Value { get; }

    private KboNummer(string? value)
    {
        Value = value;
    }

    public static KboNummer Create(string? kboNummer)
    {
        if (string.IsNullOrEmpty(kboNummer))
            return Leeg;

        var value = Sanitize(kboNummer);
        Validate(value);

        return new KboNummer(value);
    }

    public override string? ToString()
        => Value;

    public static implicit operator string?(KboNummer kboNummer)
        => kboNummer.ToString();

    /// <summary>
    ///     if kboNummer contains spaces or dots, the 5th and the 9th character are deleted
    ///     these are the only allowed positions for spaces or dots
    /// </summary>
    /// <param name="kboNummer"></param>
    /// <returns></returns>
    private static string Sanitize(string kboNummer)
        => kboNummer
            .Replace(".", "")
            .Replace(" ", "");

    private static void Validate(string value)
    {
        Throw<InvalidKboNummerChars>.IfNot(ulong.TryParse(value, out _));
        Throw<InvalidKboNummerLength>.If(value.Length != 10);
        Throw<InvalidKboNummerMod97>.IfNot(Modulo97Correct(value));
    }

    private static bool Modulo97Correct(string value)
    {
        var baseNumber = int.Parse(value[..8]);
        var remainder = int.Parse(value[8..]);

        var modulo97 = 97 - baseNumber % 97;
        return modulo97 == remainder;
    }
}
