namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using System.Text;
using Vereniging.Bronnen;

public record ErkenningWerdVerlopen(
    int ErkenningId,
    DateOnly VerlopenOp
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"ErkenningId = {ErkenningId}, ");
        builder.Append($"Verlopen = {VerlopenOp}, ");

        return true;
    }
}
