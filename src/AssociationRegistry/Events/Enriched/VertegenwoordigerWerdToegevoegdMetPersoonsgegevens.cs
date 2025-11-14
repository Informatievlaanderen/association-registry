namespace AssociationRegistry.Events.Enriched;

using AssociationRegistry.Vereniging.Bronnen;
using System.Runtime.Serialization;

public record VertegenwoordigerWerdToegevoegdMetPersoonsgegevens(
    int VertegenwoordigerId,
    bool IsPrimair,
    VertegenwoordigerPersoonsgegevens? VertegenwoordigerPersoonsgegevens) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
};
