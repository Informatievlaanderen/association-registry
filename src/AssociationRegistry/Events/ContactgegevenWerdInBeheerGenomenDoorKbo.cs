namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record ContactgegevenWerdInBeheerGenomenDoorKbo(
    int ContactgegevenId,
    string Contactgegeventype,
    string TypeVolgensKbo,
    string Waarde) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;


}
