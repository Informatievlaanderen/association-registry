namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record ContactgegevenUitKBOWerdGewijzigd(
    int ContactgegevenId,
    string Beschrijving,
    bool IsPrimair) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;

}
