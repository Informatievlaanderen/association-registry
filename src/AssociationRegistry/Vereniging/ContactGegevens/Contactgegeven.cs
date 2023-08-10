namespace AssociationRegistry.Vereniging;

using Framework;
using Exceptions;

public class Contactgegeven
{
    public int ContactgegevenId { get; init; }
    public ContactgegevenType Type { get; init; }
    public IContactWaarde Waarde { get; }
    public string Beschrijving { get; init; }
    public bool IsPrimair { get; init; }

    private Contactgegeven(int contactgegevenId, ContactgegevenType type, IContactWaarde waarde, string beschrijving, bool isPrimair)
    {
        ContactgegevenId = contactgegevenId;
        Type = type;
        Waarde = waarde;
        Beschrijving = beschrijving;
        IsPrimair = isPrimair;
    }

    public static Contactgegeven Hydrate(int contactgegevenId, ContactgegevenType type, string waarde, string beschrijving, bool isPrimair)
        => new(contactgegevenId, type, type.Hydrate(waarde), beschrijving, isPrimair);

    public static Contactgegeven Create(ContactgegevenType type, string waarde, string? beschrijving, bool isPrimair)
    {
        beschrijving ??= string.Empty;

        return new Contactgegeven(0, type, type.Create(waarde), beschrijving, isPrimair);
    }

    public Contactgegeven WithId(int contactgegevenId)
        => new(contactgegevenId, Type, Waarde, Beschrijving, IsPrimair);

    public static Contactgegeven Create(string type, string waarde, string? beschrijving, bool isPrimair)
    {
        Throw<InvalidContactType>.IfNot(IsKnownType(type));
        return Create(ContactgegevenType.Parse(type), waarde, beschrijving, isPrimair);
    }

    public bool IsEquivalentTo(Contactgegeven contactgegeven)
        => Type == contactgegeven.Type &&
           Waarde.Equals(contactgegeven.Waarde) &&
           Beschrijving == contactgegeven.Beschrijving;


    private static bool IsKnownType(string type)
        => ContactgegevenType.CanParse(type);

    public Contactgegeven CopyWithValuesIfNotNull(string? waarde, string? beschrijving, bool? isPrimair)
        => Create(Type, waarde ?? Waarde.Waarde, beschrijving ?? Beschrijving, isPrimair ?? IsPrimair).WithId(ContactgegevenId);

    public bool Equals(Contactgegeven? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ContactgegevenId == other.ContactgegevenId &&
               Type.Equals(other.Type) &&
               Waarde.Equals(other.Waarde) &&
               Beschrijving == other.Beschrijving &&
               IsPrimair == other.IsPrimair;
    }

    public override int GetHashCode()
        => HashCode.Combine(ContactgegevenId, Type, Waarde, Beschrijving, IsPrimair);

    public bool WouldBeEquivalent(string? waarde, string? beschrijving, bool? isPrimair, out Contactgegeven contactgegeven)
    {
        contactgegeven = CopyWithValuesIfNotNull(waarde, beschrijving, isPrimair);
        return Equals(contactgegeven);
    }
}
