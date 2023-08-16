namespace AssociationRegistry.Vereniging;

public sealed class ContactgegevenTypeVolgensKbo : IEquatable<ContactgegevenTypeVolgensKbo>
{
    public static class Labels
    {
        public const string Email = "E-mail";
        public const string Website = "Website";
        public const string SocialMedia = "SocialMedia";
        public const string Telefoon = "Telefoon";
        public const string GSM = "GSM";

    }

    public static readonly ContactgegevenTypeVolgensKbo Email =
        new(
            Labels.Email,
            ContactgegevenType.Email
        );

    public static readonly ContactgegevenTypeVolgensKbo Website =
        new(
            Labels.Website,
            ContactgegevenType.Website
        );

    public static readonly ContactgegevenTypeVolgensKbo SocialMedia =
        new(
            Labels.SocialMedia,
            ContactgegevenType.SocialMedia
        );

    public static readonly ContactgegevenTypeVolgensKbo Telefoon =
        new(
            Labels.Telefoon,
            ContactgegevenType.Telefoon
        );

    public static readonly ContactgegevenTypeVolgensKbo GSM =
        new(
            Labels.GSM,
            ContactgegevenType.Telefoon
        );

    public static readonly ContactgegevenTypeVolgensKbo[] All =
    {
        Email, Website, SocialMedia, Telefoon,
    };

    public string Waarde { get; }
    public ContactgegevenType ContactgegevenType { get; }

    private ContactgegevenTypeVolgensKbo(string waarde, ContactgegevenType contactgegevenType)
    {
        Waarde = waarde;
        ContactgegevenType = contactgegevenType;
    }


    public static bool CanParse(string? value)
    {
        if (value is null)
            return false;

        return Array.Find(All, candidate => candidate == value) is not null;
    }

    public static bool TryParse(string? value, out ContactgegevenTypeVolgensKbo? parsed)
    {
        parsed = null;
        if (value is null)
            return false;

        parsed = Array.Find(All, candidate => candidate == value) ?? null;
        return parsed is not null;
    }

    public static ContactgegevenTypeVolgensKbo Parse(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (!TryParse(value, out var parsed))
        {
            throw new FormatException($"De waarde {value} is geen gekend contactgegeven type.");
        }

        return parsed!;
    }

    public bool Equals(ContactgegevenTypeVolgensKbo? other)
        => other is not null && string.Equals(other.Waarde, Waarde, StringComparison.InvariantCultureIgnoreCase);

    public override bool Equals(object? obj)
        => obj is ContactgegevenTypeVolgensKbo type && Equals(type);

    public override int GetHashCode()
        => Waarde.GetHashCode();

    public override string ToString()
        => Waarde;

    public static implicit operator string(ContactgegevenTypeVolgensKbo instance)
        => instance.ToString();

    public static implicit operator ContactgegevenTypeVolgensKbo(string instance)
        => Parse(instance);

    public static bool operator ==(ContactgegevenTypeVolgensKbo left, ContactgegevenTypeVolgensKbo right)
        => Equals(left, right);

    public static bool operator !=(ContactgegevenTypeVolgensKbo left, ContactgegevenTypeVolgensKbo right)
        => !(left == right);

    public static bool operator ==(ContactgegevenTypeVolgensKbo left, string right)
        => string.Equals(left.Waarde, right, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(ContactgegevenTypeVolgensKbo left, string right)
        => !(left == right);

    public static bool operator ==(string left, ContactgegevenTypeVolgensKbo right)
        => string.Equals(left, right.Waarde, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(string left, ContactgegevenTypeVolgensKbo right)
        => !(left == right);
}
