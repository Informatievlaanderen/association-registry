namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using System.Text;
using Vereniging.Bronnen;

public record ErkenningWerdVerlengd(
    int ErkenningId,
    DateOnly Einddatum,
    DateOnly? Hernieuwingsdatum,
    string Status
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"ErkenningId = {ErkenningId}, ");
        builder.Append($"Einddatum = {Einddatum}, ");
        builder.Append($"Hernieuwingsdatum = {Hernieuwingsdatum}, ");
        builder.Append($"Status = {Status}, ");

        return true;
    }
}
