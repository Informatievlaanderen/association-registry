namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record BankrekeningnummerWerdToegevoegd(
    int BankrekeningnummerId,
    string Iban,
    string Doel,
    string Titularis) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}

