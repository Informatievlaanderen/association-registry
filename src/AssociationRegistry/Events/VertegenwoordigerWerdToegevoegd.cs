namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record VertegenwoordigerWerdToegevoegd(
    Guid RefId,
    int VertegenwoordigerId,
    bool IsPrimair) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;


}
