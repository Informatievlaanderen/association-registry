namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record ContactgegevenWerdToegevoegd(
    int ContactgegevenId,
    string Contactgegeventype,
    string Waarde,
    string Beschrijving,
    bool IsPrimair) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;


}
