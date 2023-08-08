namespace AssociationRegistry.Vereniging;

public sealed class ContactgegevenType : IEquatable<ContactgegevenType>
{
    public static class Labels
    {
        public const string Email = "E-mail";
        public const string Website = "Website";
        public const string SocialMedia = "SocialMedia";
        public const string Telefoon = "Telefoon";

    }

    public static readonly ContactgegevenType Email =
        new(
            Labels.Email
        );

    public static readonly ContactgegevenType Website =
        new(
            Labels.Website
        );

    public static readonly ContactgegevenType SocialMedia =
        new(
            Labels.SocialMedia
        );

    public static readonly ContactgegevenType Telefoon =
        new(
            Labels.Telefoon
        );

    public static readonly ContactgegevenType[] All =
    {
        Email, Website, SocialMedia, Telefoon,
    };

    public string Waarde { get; }

    private ContactgegevenType(string value)
    {
        Waarde = value;
    }


    public static bool CanParse(string? value)
    {
        if (value is null)
            return false;

        return Array.Find(All, candidate => candidate == value) is not null;
    }

    public static bool TryParse(string? value, out ContactgegevenType? parsed)
    {
        parsed = null;
        if (value is null)
            return false;

        parsed = Array.Find(All, candidate => candidate == value) ?? null;
        return parsed is not null;
    }

    public static ContactgegevenType Parse(string value)
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

    public bool Equals(ContactgegevenType? other)
        => other is not null && string.Equals(other.Waarde, Waarde, StringComparison.InvariantCultureIgnoreCase);

    public override bool Equals(object? obj)
        => obj is ContactgegevenType type && Equals(type);

    public override int GetHashCode()
        => Waarde.GetHashCode();

    public override string ToString()
        => Waarde;

    public static implicit operator string(ContactgegevenType instance)
        => instance.ToString();

    public static implicit operator ContactgegevenType(string instance)
        => Parse(instance);

    public static bool operator ==(ContactgegevenType left, ContactgegevenType right)
        => Equals(left, right);

    public static bool operator !=(ContactgegevenType left, ContactgegevenType right)
        => !(left == right);

    public static bool operator ==(ContactgegevenType left, string right)
        => string.Equals(left.Waarde, right, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(ContactgegevenType left, string right)
        => !(left == right);

    public static bool operator ==(string left, ContactgegevenType right)
        => string.Equals(left, right.Waarde, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(string left, ContactgegevenType right)
        => !(left == right);
}
