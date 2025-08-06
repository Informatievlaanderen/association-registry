namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public sealed class Contactgegeventype : IEquatable<Contactgegeventype>
{
    public static readonly Contactgegeventype Email =
        new(
            Labels.Email
        );

    public static readonly Contactgegeventype Website =
        new(
            Labels.Website
        );

    public static readonly Contactgegeventype SocialMedia =
        new(
            Labels.SocialMedia
        );

    public static readonly Contactgegeventype Telefoon =
        new(
            Labels.Telefoon
        );

    public static readonly Contactgegeventype[] All =
    {
        Email, Website, SocialMedia, Telefoon,
    };

    private Contactgegeventype(string value)
    {
        Waarde = value;
    }

    public string Waarde { get; }

    public bool Equals(Contactgegeventype? other)
        => other is not null && string.Equals(other.Waarde, Waarde, StringComparison.InvariantCultureIgnoreCase);

    public static bool CanParse(string? value)
    {
        if (value is null)
            return false;

        return Array.Find(All, match: candidate => candidate == value) is not null;
    }

    public static bool TryParse(string? value, out Contactgegeventype? parsed)
    {
        parsed = null;

        if (value is null)
            return false;

        parsed = Array.Find(All, match: candidate => candidate == value) ?? null;

        return parsed is not null;
    }

    public static Contactgegeventype Parse(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (!TryParse(value, out var parsed))
            throw new FormatException($"De waarde {value} is geen gekend contactgegeven type.");

        return parsed!;
    }

    public override bool Equals(object? obj)
        => obj is Contactgegeventype type && Equals(type);

    public override int GetHashCode()
        => Waarde.GetHashCode();

    public override string ToString()
        => Waarde;

    public static implicit operator string(Contactgegeventype instance)
        => instance.ToString();

    public static implicit operator Contactgegeventype(string instance)
        => Parse(instance);

    public static bool operator ==(Contactgegeventype left, Contactgegeventype right)
        => Equals(left, right);

    public static bool operator !=(Contactgegeventype left, Contactgegeventype right)
        => !(left == right);

    public static bool operator ==(Contactgegeventype left, string right)
        => string.Equals(left.Waarde, right, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(Contactgegeventype left, string right)
        => !(left == right);

    public static bool operator ==(string left, Contactgegeventype right)
        => string.Equals(left, right.Waarde, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(string left, Contactgegeventype right)
        => !(left == right);

    public static class Labels
    {
        public const string Email = "E-mail";
        public const string Website = "Website";
        public const string SocialMedia = "SocialMedia";
        public const string Telefoon = "Telefoon";
    }
}
