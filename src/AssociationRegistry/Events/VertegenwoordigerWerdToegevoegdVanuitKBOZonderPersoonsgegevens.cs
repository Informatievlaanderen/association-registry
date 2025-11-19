namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevens(
    Guid RefId,
    int VertegenwoordigerId) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;
}
