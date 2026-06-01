namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record ErkenningWerdGeactiveerd(int ErkenningId) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"ErkenningId = {ErkenningId}, ");

        return true;
    }
}
