namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt(
    int BankrekeningnummerId,
    string GeannuleerdDoor
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"BankrekeningnummerId = {BankrekeningnummerId}, ");
        builder.Append($"GeannuleerdDoor = {GeannuleerdDoor}");
        return true;
    }
}
