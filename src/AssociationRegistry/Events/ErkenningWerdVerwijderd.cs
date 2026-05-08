namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using System.Text;
using Vereniging.Bronnen;

public record ErkenningWerdVerwijderd(int ErkenningId) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"ErkenningId = {ErkenningId}, ");

        return true;
    }
}
