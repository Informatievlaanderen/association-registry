namespace AssociationRegistry.Events.Enriched;

using AssociationRegistry.Vereniging.Bronnen;
using System.Runtime.Serialization;

public record VertegenwoordigerWerdGewijzigdMetPersoonsgegevens(
    int VertegenwoordigerId,
    bool IsPrimair,
    VertegenwoordigerPersoonsgegevens? VertegenwoordigerPersoonsgegevens) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
};
