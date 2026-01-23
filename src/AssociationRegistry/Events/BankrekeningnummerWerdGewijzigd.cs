namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

[Obsolete("These are the upcasted events, only use this in projections and State")]
public record BankrekeningnummerWerdGewijzigd(
    int BankrekeningnummerId,
    string Doel,
    string Titularis) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}

public record BankrekeningnummerWerdGewijzigdZonderPersoonsgegevens(
    Guid RefId,
    int BankrekeningnummerId,
    string Doel) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}

