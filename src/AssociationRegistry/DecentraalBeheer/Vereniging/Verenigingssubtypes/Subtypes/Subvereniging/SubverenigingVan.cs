namespace AssociationRegistry.DecentraalBeheer.Vereniging.Subtypes.Subvereniging;

using Events;
using Events.Factories;

public record SubverenigingVan()
{
    public string AndereVereniging { get; set; }
    public string AndereVerenigingNaam { get; set; }
    public string Beschrijving { get; set; }
    public string Identificatie { get; set; }

    public static SubverenigingVan Hydrate(VerenigingssubtypeWerdVerfijndNaarSubvereniging @event) =>
        new()
        {
            AndereVereniging = @event.SubverenigingVan.AndereVereniging,
            AndereVerenigingNaam = @event.SubverenigingVan.AndereVerenigingNaam,
            Beschrijving = @event.SubverenigingVan.Beschrijving,
            Identificatie = @event.SubverenigingVan.Identificatie,
        };

    // Each event changes only its own fields; the other fields of the relatie stay as they are.
    public SubverenigingVan Apply(SubverenigingDetailsWerdenGewijzigd @event) =>
        this with
        {
            Beschrijving = @event.Beschrijving,
            Identificatie = @event.Identificatie,
        };

    public SubverenigingVan Apply(SubverenigingRelatieWerdGewijzigd @event) =>
        this with
        {
            AndereVereniging = @event.AndereVereniging,
            AndereVerenigingNaam = @event.AndereVerenigingNaam,
        };

    public IEvent[] Wijzig(VCode vCode, SubverenigingVanDto subverenigingVan)
    {
        IEvent[] events = [];

        if (HasRelatieChanges(subverenigingVan))
            events = events
                .Append(
                    EventFactory.SubverenigingRelatieWerdGewijzigd(
                        vCode,
                        subverenigingVan.AndereVereniging!,
                        subverenigingVan.AndereVerenigingNaam!
                    )
                )
                .ToArray();

        if (HasDetailChanges(subverenigingVan))
        {
            var identificatie = subverenigingVan.Identificatie ?? Identificatie;
            var beschrijving = subverenigingVan.Beschrijving ?? Beschrijving;

            events = events
                .Append(
                    EventFactory.DetailGegevensVanDeSubverenigingRelatieWerdenGewijzigd(
                        vCode,
                        identificatie,
                        beschrijving
                    )
                )
                .ToArray();
        }

        return events;
    }

    private bool HasRelatieChanges(SubverenigingVanDto commandSubverenigingVan) =>
        commandSubverenigingVan.AndereVereniging is not null
        && AndereVereniging != commandSubverenigingVan.AndereVereniging;

    private bool HasDetailChanges(SubverenigingVanDto commandSubverenigingVan) =>
        commandSubverenigingVan.Beschrijving is not null && Beschrijving != commandSubverenigingVan.Beschrijving
        || commandSubverenigingVan.Identificatie is not null && Identificatie != commandSubverenigingVan.Identificatie;
}
