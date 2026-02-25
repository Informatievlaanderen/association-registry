namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd(
    int BankrekeningnummerId,
    string BevestigdDoor
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"BankrekeningnummerId = {BankrekeningnummerId}, ");
        builder.Append($"GevalideerdDoor = {BevestigdDoor}");
        return true;
    }
}
