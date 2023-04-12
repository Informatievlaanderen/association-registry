namespace AssociationRegistry.Events;

using ContactGegevens;
using Framework;

public record ContactgegevenWerdGewijzigd(int ContactgegevenId, string Type, string Waarde, string Omschrijving, bool IsPrimair) : IEvent
{
    public static ContactgegevenWerdGewijzigd With(Contactgegeven contactgegeven)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Type,
            contactgegeven.Waarde,
            contactgegeven.Omschrijving,
            contactgegeven.IsPrimair);
}
