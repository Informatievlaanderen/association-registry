namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;
using Vereniging;
using Vereniging.Bronnen;

public record ContactgegevenWerdOvergenomenUitKBO(
    int ContactgegevenId,
    string Type,
    string TypeVolgensKbo,
    string Waarde) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;

    public static ContactgegevenWerdOvergenomenUitKBO With(Contactgegeven contactgegeven, ContactgegeventypeVolgensKbo typeVolgensKbo)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Contactgegeventype,
            typeVolgensKbo.Waarde,
            contactgegeven.Waarde);
}
