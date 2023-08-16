namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record ContactgegevenWerdOvergenomenUitKBO(
    int ContactgegevenId,
    string Type,
    string TypeVolgensKbo,
    string Waarde) : IEvent
{
    public static ContactgegevenWerdOvergenomenUitKBO With(Contactgegeven contactgegeven, ContactgegevenTypeVolgensKbo typeVolgensKbo)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Type,
            typeVolgensKbo.Waarde,
            contactgegeven.Waarde);
}
