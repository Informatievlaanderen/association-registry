namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using System.Text;
using Vereniging.Bronnen;

public record RedenVanSchorsingWerdGecorrigeerd(int ErkenningId, string RedenSchorsing) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"ErkenningId = {ErkenningId}, ");
        builder.Append($"RedenSchorsing = {RedenSchorsing}, ");

        return true;
    }
}
