﻿namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Framework;
using Vereniging;

public record ContactgegevenWerdOvergenomenUitKBO(
    int ContactgegevenId,
    string Type,
    string TypeVolgensKbo,
    string Waarde) : IEvent
{
    [IgnoreDataMember]
    public string Bron
        => AssociationRegistry.Vereniging.Bronnen.Bron.KBO;
    public static ContactgegevenWerdOvergenomenUitKBO With(Contactgegeven contactgegeven, ContactgegevenTypeVolgensKbo typeVolgensKbo)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Type,
            typeVolgensKbo.Waarde,
            contactgegeven.Waarde);
}
