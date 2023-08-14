namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record ContactgegevenWerdOvergenomenUitKBO(
    int ContactgegevenId,
    string Type,
    string Waarde) : IEvent
{
    public static ContactgegevenWerdOvergenomenUitKBO With(Contactgegeven contactgegeven)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Type,
            contactgegeven.Waarde);
}
