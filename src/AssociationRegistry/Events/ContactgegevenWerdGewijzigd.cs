namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record ContactgegevenWerdGewijzigd(
    int ContactgegevenId,
    string Type,
    string Waarde,
    string Beschrijving,
    bool IsPrimair) : IEvent
{
    public static ContactgegevenWerdGewijzigd With(Contactgegeven contactgegeven)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Type,
            contactgegeven.Waarde.Waarde,
            contactgegeven.Beschrijving,
            contactgegeven.IsPrimair);
}
