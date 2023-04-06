namespace AssociationRegistry.ContactGegevens;

using Emails;
using Exceptions;
using Framework;
using SocialMedias;
using TelefoonNummers;
using Websites;

public record Contactgegeven
{
    public int ContactgegevenId { get; init; }
    public ContactgegevenType Type { get; }
    public string Waarde { get; }
    public string Omschrijving { get; }
    public bool IsPrimair { get; }

    protected Contactgegeven(ContactgegevenType type, string waarde, string omschrijving, bool isPrimair)
    {
        ContactgegevenId = 0;
        Type = type;
        Waarde = waarde;
        Omschrijving = omschrijving;
        IsPrimair = isPrimair;
    }

    private Contactgegeven(int contactgegevenId, ContactgegevenType type, string waarde, string omschrijving, bool isPrimair)
    {
        ContactgegevenId = contactgegevenId;
        Type = type;
        Waarde = waarde;
        Omschrijving = omschrijving;
        IsPrimair = isPrimair;
    }

    public static Contactgegeven FromEvent(int contactgegevenId, ContactgegevenType type, string waarde, string omschrijving, bool isPrimair)
        => new(contactgegevenId, type, waarde, omschrijving, isPrimair);

    public static Contactgegeven Create(ContactgegevenType type, string waarde, string? omschrijving, bool isPrimair)
    {
        omschrijving ??= string.Empty;

        return type switch
        {
            { Waarde: nameof(ContactgegevenType.Email) } => Email.Create(waarde, omschrijving, isPrimair),
            { Waarde: nameof(ContactgegevenType.Telefoon) } => TelefoonNummer.Create(waarde, omschrijving, isPrimair),
            { Waarde: nameof(ContactgegevenType.Website) } => Website.Create(waarde, omschrijving, isPrimair),
            { Waarde: nameof(ContactgegevenType.SocialMedia) } => SocialMedia.Create(waarde, omschrijving, isPrimair),
            _ => throw new InvalidContactType(),
        };
    }

    public bool MetZelfdeWaarden(Contactgegeven contactgegeven)
    {
        return Type == contactgegeven.Type && Waarde == contactgegeven.Waarde;
    }

    public static Contactgegeven Create(string type, string waarde, string? omschrijving, bool isPrimair)
    {
        Throw<InvalidContactType>.IfNot(IsKnownType(type));
        return Create(ContactgegevenType.Parse(type), waarde, omschrijving, isPrimair);
    }

    private static bool IsKnownType(string type)
        => ContactgegevenType.CanParse(type);

    public Contactgegeven MetWaarden(string? waarde, string? omschrijving, bool? isPrimair)
        => Create(Type, waarde ?? Waarde, omschrijving ?? Omschrijving, isPrimair ?? IsPrimair) with { ContactgegevenId = ContactgegevenId };

    public virtual bool Equals(Contactgegeven? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ContactgegevenId == other.ContactgegevenId &&
               Type.Equals(other.Type) &&
               Waarde == other.Waarde &&
               Omschrijving == other.Omschrijving &&
               IsPrimair == other.IsPrimair;
    }

    public override int GetHashCode()
        => HashCode.Combine(ContactgegevenId, Type, Waarde, Omschrijving, IsPrimair);
}
