namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using DecentraalBeheer.Vereniging.Erkenningen;
using Vereniging.Bronnen;

public record ErkenningWerdGeregistreerd(
    int ErkenningId,
    string VCode,
    IpdcProduct IpdcProduct,
    DateOnly Startdatum,
    DateOnly Einddatum,
    DateOnly Hernieuwingsdatum,
    string HernieuwingsUrl,
    GegevensInitiator GeregistreerdDoor
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.Initiator;

    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"ErkenningId = {ErkenningId}, ");
        builder.Append($"IpdcProduct = {IpdcProduct}, ");
        builder.Append($"Startdatum = {Startdatum}, ");
        builder.Append($"Einddatum = {Einddatum}, ");
        builder.Append($"Hernieuwingsdatum = {Hernieuwingsdatum}, ");
        builder.Append($"HernieuwingsUrl = {HernieuwingsUrl}, ");
        builder.Append($" GeregistreerdDoor = {GeregistreerdDoor}, ");

        return true;
    }
}
