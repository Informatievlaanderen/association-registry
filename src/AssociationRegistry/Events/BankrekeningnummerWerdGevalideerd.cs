namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

[Obsolete("These are the upcasted events, only use this in projections and State")]
public record BankrekeningnummerWerdGevalideerd(
    int BankrekeningnummerId,
    string Iban,
    string Titularis,
    string GevalideerdDoor
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"BankrekeningnummerId = {BankrekeningnummerId}, ");
        builder.Append($"GevalideerdDoor = {GevalideerdDoor}");
        return true;
    }
}

public record BankrekeningnummerWerdGevalideerdZonderPersoonsgegevens(
    Guid RefId,
    int BankrekeningnummerId,
    string GevalideerdDoor
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;
}
