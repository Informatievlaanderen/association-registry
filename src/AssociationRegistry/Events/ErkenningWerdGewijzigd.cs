namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record ErkenningWerdGewijzigd(
    int ErkenningId,
    DateOnly? Startdatum,
    DateOnly? Einddatum,
    DateOnly? Hernieuwingsdatum,
    string? HernieuwingsUrl,
    string? Status,
    string RedenVanWijziging
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
        builder.Append($"Status = {Status}, ");
        builder.Append($"RedenVanWijziging = {RedenVanWijziging}, ");

        return true;
    }
}
