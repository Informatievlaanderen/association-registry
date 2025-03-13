namespace AssociationRegistry.Vereniging;

using DecentraalBeheer.Subtype;
using EventFactories;
using Events;

public record SubverenigingVan(VCode vCode)
{
    public VCode VCode { get; } = vCode;
    public string AndereVereniging { get; set; }
    public string AndereVerenigingNaam { get; set; }
    public string Beschrijving { get; set; }
    public string Identificatie { get; set; }

    public SubverenigingVan Hydrate(VerenigingssubtypeWerdVerfijndNaarSubvereniging @event)
        => new(VCode)
        {
            AndereVereniging = @event.SubverenigingVan.AndereVereniging,
            AndereVerenigingNaam = @event.SubverenigingVan.AndereVerenigingNaam,
            Beschrijving = @event.SubverenigingVan.Beschrijving,
            Identificatie = @event.SubverenigingVan.Identificatie,
        };

    public SubverenigingVan Hydrate(DetailGegevensVanDeSubverenigingRelatieWerdenGewijzigd @event)
        => new(VCode)
        {
            Beschrijving = @event.Beschrijving,
            Identificatie = @event.Identificatie,
        };

    public SubverenigingVan Hydrate(SubverenigingRelatieWerdGewijzigd @event)
        => new(VCode)
        {
            AndereVereniging = @event.AndereVereniging,
            AndereVerenigingNaam = @event.AndereVerenigingNaam,
        };

    public static SubverenigingVan Create(VCode vCode)
        => new(vCode);

    public IEvent[] Verfijn(VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan subverenigingVan)
        =>
        [
            EventFactory.VerenigingssubtypeWerdVerfijndNaarSubvereniging(
                VCode,
                subverenigingVan.AndereVereniging!,
                subverenigingVan.AndereVerenigingNaam!,
                subverenigingVan.Identificatie!,
                subverenigingVan.Beschrijving!),
        ];

    public IEvent[] Wijzig(VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan commandSubverenigingVan)
    {
        IEvent[] events = [];

        if (HasRelatieChanges(commandSubverenigingVan))
            events = events.Append(EventFactory.SubverenigingRelatieWerdGewijzigd(VCode, commandSubverenigingVan.AndereVereniging!, commandSubverenigingVan.AndereVerenigingNaam!)).ToArray();

        if (HasDetailChanges(commandSubverenigingVan))
        {
            var identificatie = commandSubverenigingVan.Identificatie ?? Identificatie;
            var beschrijving = commandSubverenigingVan.Beschrijving ?? Beschrijving;

            events = events.Append(EventFactory.DetailGegevensVanDeSubverenigingRelatieWerdenGewijzigd(VCode, identificatie, beschrijving)).ToArray();
        }

        return events;
    }

    private bool HasRelatieChanges(VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan commandSubverenigingVan)
        => commandSubverenigingVan.AndereVereniging is not null && AndereVereniging != commandSubverenigingVan.AndereVereniging;

    private bool HasDetailChanges(VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan commandSubverenigingVan)
        => commandSubverenigingVan.Beschrijving is not null && Beschrijving != commandSubverenigingVan.Beschrijving ||
           commandSubverenigingVan.Identificatie is not null && Identificatie != commandSubverenigingVan.Identificatie;
}
