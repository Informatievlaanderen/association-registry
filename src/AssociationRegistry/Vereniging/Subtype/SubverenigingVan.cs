namespace AssociationRegistry.Vereniging;

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

    public IEvent[] Verfijn(DecentraalBeheer.Subtype.SubverenigingVan subverenigingVan)
        => [EventFactory.VerenigingssubtypeWerdVerfijndNaarSubvereniging(VCode, ToAndereVerenging(subverenigingVan))];

    public IEvent[] Wijzig(DecentraalBeheer.Subtype.SubverenigingVan subverenigingVan)
    {
        IEvent[] events = [];

        if (HasRelatieChanges(subverenigingVan))
        {
            var teWijzigen = ToWijzigRelatie(subverenigingVan);
            events = events.Append(EventFactory.SubverenigingRelatieWerdGewijzigd(VCode,teWijzigen)).ToArray();
        }

        if (HasDetailChanges(subverenigingVan))
        {
            var teWijzigen = ToWijzigDetail(subverenigingVan);
            events = events.Append(EventFactory.DetailGegevensVanDeSubverenigingRelatieWerdenGewijzigd(VCode,teWijzigen)).ToArray();
        }

        return events;
    }

    public SubverenigingVan ToAndereVerenging(DecentraalBeheer.Subtype.SubverenigingVan subverenigingVan)
        => this with
        {
            AndereVereniging =  subverenigingVan.AndereVereniging!,
            AndereVerenigingNaam = subverenigingVan.AndereVerenigingNaam!,
            Identificatie =  subverenigingVan.Identificatie!,
            Beschrijving = subverenigingVan.Beschrijving!,
        };

    public SubverenigingVan ToWijzigRelatie(DecentraalBeheer.Subtype.SubverenigingVan subverenigingVan)
        => this with
        {
            AndereVereniging =  subverenigingVan.AndereVereniging ?? AndereVereniging,
            AndereVerenigingNaam = subverenigingVan.AndereVerenigingNaam ?? AndereVerenigingNaam,
        };

    public SubverenigingVan ToWijzigDetail(DecentraalBeheer.Subtype.SubverenigingVan subverenigingVan)
        => this with
        {
            Identificatie =  subverenigingVan.Identificatie ?? Identificatie,
            Beschrijving = subverenigingVan.Beschrijving ?? Beschrijving,
        };

    private bool HasRelatieChanges(DecentraalBeheer.Subtype.SubverenigingVan subverenigingVan)
        => subverenigingVan.AndereVereniging is not null && AndereVereniging != subverenigingVan.AndereVereniging;

    private bool HasDetailChanges(DecentraalBeheer.Subtype.SubverenigingVan subverenigingVan)
        => subverenigingVan.Beschrijving is not null && Beschrijving != subverenigingVan.Beschrijving ||
           subverenigingVan.Identificatie is not null && Identificatie != subverenigingVan.Identificatie;
}
