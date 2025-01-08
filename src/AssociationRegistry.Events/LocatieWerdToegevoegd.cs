namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging;
using Vereniging.Bronnen;

public record LocatieWerdToegevoegd(
    Registratiedata.Locatie Locatie) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;


}
