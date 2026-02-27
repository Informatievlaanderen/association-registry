namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;
[Obsolete("Feature was gedeployed on test environment but was dropped. Kept event for documentation.")]
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
        builder.Append($"OngedaanGemaaktDoor = {OngedaanGemaaktDoor}, ");
        return true;
    }
}
