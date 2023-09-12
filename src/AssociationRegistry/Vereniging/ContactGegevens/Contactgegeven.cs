namespace AssociationRegistry.Vereniging;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Bronnen;
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
    public Bron Bron { get; init; }

    protected Contactgegeven(ContactgegevenType type, string waarde, string beschrijving, bool isPrimair)
    {
        ContactgegevenId = 0;
        Type = type;
        Waarde = waarde;
        Beschrijving = beschrijving;
        IsPrimair = isPrimair;
    }

    private Contactgegeven(int contactgegevenId, ContactgegevenType type, string waarde, string beschrijving, bool isPrimair, Bron bron)
    {
        ContactgegevenId = contactgegevenId;
        Type = type;
        Waarde = waarde;
        Beschrijving = beschrijving;
        IsPrimair = isPrimair;
        Bron = bron;
    }

    public static Contactgegeven Hydrate(
        int contactgegevenId,
        ContactgegevenType type,
        string waarde,
        string beschrijving,
        bool isPrimair,
        Bron bron)
        => new(contactgegevenId, type, waarde, beschrijving, isPrimair, bron);

    public static Contactgegeven CreateFromInitiator(ContactgegevenType type, string waarde, string? beschrijving, bool isPrimair)
    {
        beschrijving ??= string.Empty;

        Contactgegeven contactgegeven = type switch
        {
            { Waarde: ContactgegevenType.Labels.Email } => Email.Create(waarde, beschrijving, isPrimair),
            { Waarde: ContactgegevenType.Labels.Telefoon } => TelefoonNummer.Create(waarde, beschrijving, isPrimair),
            { Waarde: ContactgegevenType.Labels.Website } => Website.Create(waarde, beschrijving, isPrimair),
            { Waarde: ContactgegevenType.Labels.SocialMedia } => SocialMedia.Create(waarde, beschrijving, isPrimair),
            _ => throw new InvalidContactType(),
        };

        return contactgegeven with { Bron = Bron.Initiator };
    }

    public static Contactgegeven CreateFromKbo(ContactgegevenType type, string waarde)
        => CreateFromInitiator(type, waarde, string.Empty, false) with { Bron = Bron.KBO };

    public static Contactgegeven Create(string type, string waarde, string? beschrijving, bool isPrimair)
    {
        Throw<InvalidContactType>.IfNot(IsKnownType(type));

        return CreateFromInitiator(ContactgegevenType.Parse(type), waarde, beschrijving, isPrimair);
    }

    public bool IsEquivalentTo(Contactgegeven contactgegeven)
        => Type == contactgegeven.Type &&
           Waarde == contactgegeven.Waarde &&
           Beschrijving == contactgegeven.Beschrijving;

    private static bool IsKnownType(string type)
        => ContactgegevenType.CanParse(type);

    public Contactgegeven CopyWithValuesIfNotNull(string? waarde, string? beschrijving, bool? isPrimair)
        => CreateFromInitiator(Type, waarde ?? Waarde, beschrijving ?? Beschrijving, isPrimair ?? IsPrimair) with
        {
            ContactgegevenId = ContactgegevenId,
            Bron = Bron,
        };

    public virtual bool Equals(Contactgegeven? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return ContactgegevenId == other.ContactgegevenId &&
               Type.Equals(other.Type) &&
               Waarde == other.Waarde &&
               Beschrijving == other.Beschrijving &&
               IsPrimair == other.IsPrimair &&
               Bron == other.Bron;
    }

    public override int GetHashCode()
        => HashCode.Combine(ContactgegevenId, Type, Waarde, Beschrijving, IsPrimair, Bron);

    public bool WouldBeEquivalent(string? waarde, string? beschrijving, bool? isPrimair, out Contactgegeven contactgegeven)
    {
        contactgegeven = CopyWithValuesIfNotNull(waarde, beschrijving, isPrimair);

        return this == contactgegeven;
    }

    public static Contactgegeven? TryCreateFromKbo(string waarde, ContactgegevenTypeVolgensKbo type)
    {
        try
        {
            return CreateFromKbo(type.ContactgegevenType, waarde);
        }
        catch (DomainException)
        {
            return null;
        }
    }
}
