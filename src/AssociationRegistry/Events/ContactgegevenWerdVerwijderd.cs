namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record ContactgegevenWerdVerwijderd(int ContactgegevenId, string Type, string Waarde, string Beschrijving, bool IsPrimair) : IEvent
{
    public static ContactgegevenWerdVerwijderd With(Contactgegeven contactgegeven)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Contactgegeventype,
            contactgegeven.Waarde,
            contactgegeven.Beschrijving,
            contactgegeven.IsPrimair);
}
