namespace AssociationRegistry.Vereniging;

using EventFactories;
using Events;

public class SubverenigingVan(VCode vCode)
{
    public VCode VCode { get; } = vCode;
    public string AndereVereniging { get; set; }
    public string Beschrijving { get; set; }
    public string Identificatie { get; set; }

    public SubverenigingVan Hydrate(VerenigingssubtypeWerdVerfijndNaarSubvereniging @event)
        => new(VCode)
        {
            AndereVereniging = @event.SubverenigingVan.AndereVereniging,
            Beschrijving = @event.SubverenigingVan.Beschrijving,
            Identificatie = @event.SubverenigingVan.Identificatie,
        };

    public static SubverenigingVan Create(VCode vCode)
        => new(vCode);

    public IEvent[] Wijzig(DecentraalBeheer.Subtype.SubverenigingVan subverenigingVan)
    {
        return [EventFactory.VerenigingssubtypeWerdVerfijndNaarSubvereniging(VCode,subverenigingVan)];
    }
}
