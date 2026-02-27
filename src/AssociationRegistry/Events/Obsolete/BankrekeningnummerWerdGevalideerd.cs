namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

[Obsolete("Feature was gedeployed on test environment but was dropped. Kept event for documentation.")]
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

[Obsolete("Feature was gedeployed on test environment but was dropped. Kept event for documentation.")]
public record BankrekeningnummerWerdGevalideerdZonderPersoonsgegevens(
    Guid RefId,
    int BankrekeningnummerId,
    string GevalideerdDoor
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;
}
