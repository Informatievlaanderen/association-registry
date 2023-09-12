namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;

public record ContactgegevenVolgensKBOWerdGewijzigd(
    int ContactgegevenId,
    string Beschrijving,
    bool IsPrimair) : IEvent
{
    [IgnoreDataMember]
    public string Bron
        => Vereniging.Bronnen.Bron.Initiator;
}
