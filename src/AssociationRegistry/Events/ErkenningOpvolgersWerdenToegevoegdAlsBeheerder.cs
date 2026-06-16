namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using System.Text;
using Vereniging.Bronnen;

public record ErkenningOpvolgersWerdenToegevoegdAlsBeheerder(int ErkenningId, string[] Beheerders) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"ErkenningId = {ErkenningId}, ");
        builder.Append($"Beheerders = {string.Join(", ", Beheerders)}, ");

        return true;
    }
}
