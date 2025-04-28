namespace AssociationRegistry.Vereniging.Subtypes.Subvereniging;

using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Events;

public record SubverenigingVan()
{
    public string AndereVereniging { get; set; }
    public string AndereVerenigingNaam { get; set; }
    public string Beschrijving { get; set; }
    public string Identificatie { get; set; }

    public static SubverenigingVan Hydrate(VerenigingssubtypeWerdVerfijndNaarSubvereniging @event)
        => new()
        {
            AndereVereniging = @event.SubverenigingVan.AndereVereniging,
            AndereVerenigingNaam = @event.SubverenigingVan.AndereVerenigingNaam,
            Beschrijving = @event.SubverenigingVan.Beschrijving,
            Identificatie = @event.SubverenigingVan.Identificatie,
        };

    public static SubverenigingVan Hydrate(SubverenigingDetailsWerdenGewijzigd @event)
        => new()
        {
            Beschrijving = @event.Beschrijving,
            Identificatie = @event.Identificatie,
        };

    public static SubverenigingVan Hydrate(SubverenigingRelatieWerdGewijzigd @event)
        => new()
        {
            AndereVereniging = @event.AndereVereniging,
            AndereVerenigingNaam = @event.AndereVerenigingNaam,
        };

    public IEvent[] Wijzig(VCode vCode, VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan subverenigingVan)
    {
        IEvent[] events = [];

        if (HasRelatieChanges(subverenigingVan))
            events = events.Append(EventFactory.SubverenigingRelatieWerdGewijzigd(vCode, subverenigingVan.AndereVereniging!, subverenigingVan.AndereVerenigingNaam!)).ToArray();

        if (HasDetailChanges(subverenigingVan))
        {
            var identificatie = subverenigingVan.Identificatie ?? Identificatie;
            var beschrijving = subverenigingVan.Beschrijving ?? Beschrijving;

            events = events.Append(EventFactory.DetailGegevensVanDeSubverenigingRelatieWerdenGewijzigd(vCode, identificatie, beschrijving)).ToArray();
        }

        return events;
    }

    private bool HasRelatieChanges(VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan commandSubverenigingVan)
        => commandSubverenigingVan.AndereVereniging is not null && AndereVereniging != commandSubverenigingVan.AndereVereniging;

    private bool HasDetailChanges(VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan commandSubverenigingVan)
        => commandSubverenigingVan.Beschrijving is not null && Beschrijving != commandSubverenigingVan.Beschrijving ||
           commandSubverenigingVan.Identificatie is not null && Identificatie != commandSubverenigingVan.Identificatie;
}

