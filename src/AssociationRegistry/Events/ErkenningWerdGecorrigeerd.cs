namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using DecentraalBeheer.Vereniging.Erkenningen;
using Vereniging.Bronnen;

public record ErkenningWerdGecorrigeerd(
    int ErkenningId,
    DateOnly? Startdatum,
    DateOnly? Einddatum,
    DateOnly? Hernieuwingsdatum,
    string HernieuwingsUrl
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"ErkenningId = {ErkenningId}, ");
        builder.Append($"Startdatum = {Startdatum}, ");
        builder.Append($"Einddatum = {Einddatum}, ");
        builder.Append($"Hernieuwingsdatum = {Hernieuwingsdatum}, ");
        builder.Append($"HernieuwingsUrl = {HernieuwingsUrl}, ");

        return true;
    }
}
