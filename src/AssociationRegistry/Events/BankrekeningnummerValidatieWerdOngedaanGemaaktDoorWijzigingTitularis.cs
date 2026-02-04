namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record BankrekeningnummerValidatieWerdOngedaanGemaaktDoorWijzigingTitularis(
    int BankrekeningnummerId,
    string OngedaanGemaaktDoor
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"BankrekeningnummerId = {BankrekeningnummerId}, ");
        return true;
    }
}
