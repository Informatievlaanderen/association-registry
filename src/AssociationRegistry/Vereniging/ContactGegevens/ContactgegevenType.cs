namespace AssociationRegistry.Vereniging;

using TelefoonNummers;

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
            Labels.Email,
            Emails.Email.Create,
            Emails.Email.Hydrate
        );

    public static readonly ContactgegevenType Website =
        new(
            Labels.Website,
            Websites.Website.Create,
            Websites.Website.Hydrate
        );

    public static readonly ContactgegevenType SocialMedia =
        new(
            Labels.SocialMedia,
            SocialMedias.SocialMedia.Create,
            SocialMedias.SocialMedia.Hydrate
        );

    public static readonly ContactgegevenType Telefoon =
        new(
            Labels.Telefoon,
            TelefoonNummer.Create,
            TelefoonNummer.Hydrate
        );

    public static readonly ContactgegevenType[] All =
    {
        Email, Website, SocialMedia, Telefoon,
    };

    public string Label { get; }
    public Func<string, IContactWaarde> Create { get; }
    public Func<string, IContactWaarde> Hydrate { get; }

    private ContactgegevenType(string label, Func<string, IContactWaarde> create, Func<string, IContactWaarde> hydrate)
    {
        Label = label;
        Create = create;
        Hydrate = hydrate;
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
        => other is not null && string.Equals(other.Label, Label, StringComparison.InvariantCultureIgnoreCase);

    public override bool Equals(object? obj)
        => obj is ContactgegevenType type && Equals(type);

    public override int GetHashCode()
        => Label.GetHashCode();

    public override string ToString()
        => Label;

    public static implicit operator string(ContactgegevenType instance)
        => instance.ToString();

    public static implicit operator ContactgegevenType(string instance)
        => Parse(instance);

    public static bool operator ==(ContactgegevenType left, ContactgegevenType right)
        => Equals(left, right);

    public static bool operator !=(ContactgegevenType left, ContactgegevenType right)
        => !(left == right);

    public static bool operator ==(ContactgegevenType left, string right)
        => string.Equals(left.Label, right, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(ContactgegevenType left, string right)
        => !(left == right);

    public static bool operator ==(string left, ContactgegevenType right)
        => string.Equals(left, right.Label, StringComparison.InvariantCultureIgnoreCase);

    public static bool operator !=(string left, ContactgegevenType right)
        => !(left == right);
}
