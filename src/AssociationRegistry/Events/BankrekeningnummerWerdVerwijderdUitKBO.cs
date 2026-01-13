namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record BankrekeningnummerWerdVerwijderdUitKBO(
    int BankrekeningnummerId,
    string Iban) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}

