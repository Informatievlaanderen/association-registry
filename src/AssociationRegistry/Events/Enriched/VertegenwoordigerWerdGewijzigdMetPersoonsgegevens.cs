namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record VertegenwoordigerWerdGewijzigdMetPersoonsgegevens(
    int VertegenwoordigerId,
    bool IsPrimair,
    VertegenwoordigerPersoonsgegevens? VertegenwoordigerPersoonsgegevens) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
};
