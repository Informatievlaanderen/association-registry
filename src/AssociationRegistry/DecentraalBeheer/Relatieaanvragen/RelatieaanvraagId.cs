namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Exceptions;
using Framework;

/// <summary>A readable id like a vCode, from its own sequence: RA0000001.</summary>
public record RelatieaanvraagId
{
    public const string Prefix = "RA";
    private const string NumberFormat = "0000000";

    private RelatieaanvraagId(long nummer)
    {
        Nummer = nummer;
    }

    /// <summary>The format of an aanvraagId, for messages: RA0000000.</summary>
    public static string Format => Prefix + NumberFormat;

    public long Nummer { get; }

    public string Value => Prefix + Nummer.ToString(NumberFormat);

    public static RelatieaanvraagId Create(long nummer) => new(nummer);

    public static RelatieaanvraagId Hydrate(string value) => new(long.Parse(value[Prefix.Length..]));

    /// <summary>Input from a request: case-insensitive, in the <see cref="Format" />.</summary>
    public static RelatieaanvraagId Parse(string? value)
    {
        var normalized = value?.Trim().ToUpperInvariant() ?? string.Empty;

        Throw<RelatieaanvraagIdIsOngeldig>.IfNot(
            normalized.Length == Format.Length
                && normalized.StartsWith(Prefix, StringComparison.Ordinal)
                && normalized[Prefix.Length..].All(char.IsAsciiDigit)
        );

        return Hydrate(normalized);
    }

    public override string ToString() => Value;
}
