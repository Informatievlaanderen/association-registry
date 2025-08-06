namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public sealed class Locatietype : IEquatable<Locatietype>
{
    public static readonly Locatietype Correspondentie =
        new(
            "Correspondentie"
        );

    public static readonly Locatietype Activiteiten =
        new(
            "Activiteiten"
        );

    public static readonly Locatietype MaatschappelijkeZetelVolgensKbo =
        new(
            "Maatschappelijke zetel volgens KBO"
        );

    public static readonly Locatietype[] All = { Correspondentie, Activiteiten, MaatschappelijkeZetelVolgensKbo };
    public string Waarde { get; }

    private Locatietype(string value)
    {
        Waarde = value;
    }

    public static bool CanParse(string? value)
    {
        if (value is null)
            return false;

        return Array.Find(All, match: candidate => candidate == value) is not null;
    }

    public static bool TryParse(string? value, out Locatietype? parsed)
    {
        parsed = null;

        if (value is null)
            return false;

        parsed = Array.Find(All, match: candidate => candidate == value) ?? null;

        return parsed is not null;
    }

    public static Locatietype Parse(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (!TryParse(value, out var parsed))
            throw new FormatException($"De waarde {value} is geen gekend locatie type.");

        return parsed!;
    }

    public bool Equals(Locatietype? other)
        => other is not null && string.Equals(other.Waarde, Waarde, StringComparison.InvariantCultureIgnoreCase);

    public override bool Equals(object? obj)
        => obj is Locatietype type && Equals(type);

    public override int GetHashCode()
        => Waarde.GetHashCode();

    public override string ToString()
        => Waarde;

    public static implicit operator string(Locatietype instance)
        => instance.ToString();

    public static implicit operator Locatietype(string instance)
        => Parse(instance);

    public static bool operator ==(Locatietype left, Locatietype right)
        => Equals(left, right);

    public static bool operator !=(Locatietype left, Locatietype right)
        => !(left == right);

    public static bool operator ==(Locatietype left, string right)
        => string.Equals(left.Waarde, right, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(Locatietype left, string right)
        => !(left == right);

    public static bool operator ==(string left, Locatietype right)
        => string.Equals(left, right.Waarde, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(string left, Locatietype right)
        => !(left == right);
}
