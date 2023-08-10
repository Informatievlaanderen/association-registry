namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record ContactgegevenWerdToegevoegd(
    int ContactgegevenId,
    string Type,
    string Waarde,
    string Beschrijving,
    bool IsPrimair) : IEvent
{
    public static ContactgegevenWerdToegevoegd With(Contactgegeven contactgegeven)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Type,
            contactgegeven.Waarde.Waarde,
            contactgegeven.Beschrijving,
            contactgegeven.IsPrimair);
}
