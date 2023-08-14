namespace AssociationRegistry.Vereniging;

using Framework;
using Emails;
using Exceptions;
using SocialMedias;
using TelefoonNummers;
using Websites;

public record Contactgegeven
{
    public int ContactgegevenId { get; init; }
    public ContactgegevenType Type { get; init; }
    public string Waarde { get; }
    public string Beschrijving { get; init; }
    public bool IsPrimair { get; init; }

    protected Contactgegeven(ContactgegevenType type, string waarde, string beschrijving, bool isPrimair)
    {
        ContactgegevenId = 0;
        Type = type;
        Waarde = waarde;
        Beschrijving = beschrijving;
        IsPrimair = isPrimair;
    }

    private Contactgegeven(int contactgegevenId, ContactgegevenType type, string waarde, string beschrijving, bool isPrimair)
    {
        ContactgegevenId = contactgegevenId;
        Type = type;
        Waarde = waarde;
        Beschrijving = beschrijving;
        IsPrimair = isPrimair;
    }

    public static Contactgegeven Hydrate(int contactgegevenId, ContactgegevenType type, string waarde, string beschrijving, bool isPrimair)
        => new(contactgegevenId, type, waarde, beschrijving, isPrimair);

    public static Contactgegeven Create(ContactgegevenType type, string waarde, string? beschrijving, bool isPrimair)
    {
        beschrijving ??= string.Empty;

        return type switch
        {
            { Waarde: ContactgegevenType.Labels.Email } => Email.Create(waarde, beschrijving, isPrimair),
            { Waarde: ContactgegevenType.Labels.Telefoon } => TelefoonNummer.Create(waarde, beschrijving, isPrimair),
            { Waarde: ContactgegevenType.Labels.Website } => Website.Create(waarde, beschrijving, isPrimair),
            { Waarde: ContactgegevenType.Labels.SocialMedia } => SocialMedia.Create(waarde, beschrijving, isPrimair),
            _ => throw new InvalidContactType(),
        };
    }

    public static Contactgegeven Create(ContactgegevenType type, string waarde)
        => Create(type, waarde, string.Empty, false);


    public static Contactgegeven Create(string type, string waarde, string? beschrijving, bool isPrimair)
    {
        Throw<InvalidContactType>.IfNot(IsKnownType(type));
        return Create(ContactgegevenType.Parse(type), waarde, beschrijving, isPrimair);
    }

    public bool IsEquivalentTo(Contactgegeven contactgegeven)
        => Type == contactgegeven.Type &&
           Waarde == contactgegeven.Waarde &&
           Beschrijving == contactgegeven.Beschrijving;


    private static bool IsKnownType(string type)
        => ContactgegevenType.CanParse(type);

    public Contactgegeven CopyWithValuesIfNotNull(string? waarde, string? beschrijving, bool? isPrimair)
        => Create(Type, waarde ?? Waarde, beschrijving ?? Beschrijving, isPrimair ?? IsPrimair) with { ContactgegevenId = ContactgegevenId };

    public virtual bool Equals(Contactgegeven? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ContactgegevenId == other.ContactgegevenId &&
               Type.Equals(other.Type) &&
               Waarde == other.Waarde &&
               Beschrijving == other.Beschrijving &&
               IsPrimair == other.IsPrimair;
    }

    public override int GetHashCode()
        => HashCode.Combine(ContactgegevenId, Type, Waarde, Beschrijving, IsPrimair);

    public bool WouldBeEquivalent(string? waarde, string? beschrijving, bool? isPrimair, out Contactgegeven contactgegeven)
    {
        contactgegeven = CopyWithValuesIfNotNull(waarde, beschrijving, isPrimair);
        return this == contactgegeven;
    }
}
