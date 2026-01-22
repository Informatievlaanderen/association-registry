namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

[Obsolete("These are the upcasted events, only use this in projections and State")]
public record BankrekeningnummerWerdVerwijderd(
    int BankrekeningnummerId,
    string Iban) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}

public record BankrekeningnummerWerdVerwijderdZonderPersoonsgegevens(
    Guid RefId,
    int BankrekeningnummerId) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}

