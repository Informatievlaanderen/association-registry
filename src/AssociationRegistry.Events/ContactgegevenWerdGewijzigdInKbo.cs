﻿namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging;
using Vereniging.Bronnen;

public record ContactgegevenWerdGewijzigdInKbo(
    int ContactgegevenId,
    string Contactgegeventype,
    string TypeVolgensKbo,
    string Waarde) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;

}