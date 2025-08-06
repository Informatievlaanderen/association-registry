namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public sealed class ContactgegeventypeVolgensKbo : IEquatable<ContactgegeventypeVolgensKbo>
{
    public static readonly ContactgegeventypeVolgensKbo Email =
        new(
            Labels.Email,
            Contactgegeventype.Email
        );

    public static readonly ContactgegeventypeVolgensKbo Website =
        new(
            Labels.Website,
            Contactgegeventype.Website
        );

    public static readonly ContactgegeventypeVolgensKbo SocialMedia =
        new(
            Labels.SocialMedia,
            Contactgegeventype.SocialMedia
        );

    public static readonly ContactgegeventypeVolgensKbo Telefoon =
        new(
            Labels.Telefoon,
            Contactgegeventype.Telefoon
        );

    public static readonly ContactgegeventypeVolgensKbo GSM =
        new(
            Labels.GSM,
            Contactgegeventype.Telefoon
        );

    public static readonly ContactgegeventypeVolgensKbo[] All =
    {
        Email, Website, Telefoon, GSM,
    };

    private ContactgegeventypeVolgensKbo(string waarde, Contactgegeventype contactgegeventype)
    {
        Waarde = waarde;
        Contactgegeventype = contactgegeventype;
    }

    public string Waarde { get; }
    public Contactgegeventype Contactgegeventype { get; }

    public bool Equals(ContactgegeventypeVolgensKbo? other)
        => other is not null && string.Equals(other.Waarde, Waarde, StringComparison.InvariantCultureIgnoreCase);

    public static bool CanParse(string? value)
    {
        if (value is null)
            return false;

        return Array.Find(All, match: candidate => candidate.Waarde == value) is not null;
    }

    public static bool TryParse(string? value, out ContactgegeventypeVolgensKbo? parsed)
    {
        parsed = null;

        if (value is null)
            return false;

        parsed = Array.Find(All, match: candidate => candidate.Waarde == value) ?? null;

        return parsed is not null;
    }

    public static ContactgegeventypeVolgensKbo Parse(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (!TryParse(value, out var parsed))
            throw new FormatException($"De waarde {value} is geen gekend contactgegeven type.");

        return parsed!;
    }

    public override bool Equals(object? obj)
        => obj is ContactgegeventypeVolgensKbo type && Equals(type);

    public override int GetHashCode()
        => Waarde.GetHashCode();

    public override string ToString()
        => Waarde;

    public static implicit operator string(ContactgegeventypeVolgensKbo instance)
        => instance.ToString();

    public static implicit operator ContactgegeventypeVolgensKbo(string instance)
        => Parse(instance);

    public static bool operator ==(ContactgegeventypeVolgensKbo left, ContactgegeventypeVolgensKbo right)
        => Equals(left, right);

    public static bool operator !=(ContactgegeventypeVolgensKbo left, ContactgegeventypeVolgensKbo right)
        => !(left == right);

    public static bool operator ==(ContactgegeventypeVolgensKbo left, string right)
        => string.Equals(left.Waarde, right, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(ContactgegeventypeVolgensKbo left, string right)
        => !(left == right);

    public static bool operator ==(string left, ContactgegeventypeVolgensKbo right)
        => string.Equals(left, right.Waarde, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(string left, ContactgegeventypeVolgensKbo right)
        => !(left == right);

    public static class Labels
    {
        public const string Email = "E-mail";
        public const string Website = "Website";
        public const string SocialMedia = "SocialMedia";
        public const string Telefoon = "Telefoon";
        public const string GSM = "GSM";
    }
}
