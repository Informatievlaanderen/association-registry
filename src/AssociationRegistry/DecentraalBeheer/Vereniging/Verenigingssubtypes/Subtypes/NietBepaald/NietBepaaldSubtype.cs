namespace AssociationRegistry.DecentraalBeheer.Vereniging.Subtypes.NietBepaald;

using Subvereniging;
using Events;
using Events.Factories;

public record NietBepaaldSubtype : IVerenigingssubtype
{

    public IVerenigingssubtypeCode Code => VerenigingssubtypeCode.NietBepaald;

    public IVerenigingssubtype Apply(SubverenigingRelatieWerdGewijzigd @event)
        => this;

    public IVerenigingssubtype Apply(SubverenigingDetailsWerdenGewijzigd @event)
        => this;

    public IEvent[] VerFijnNaarFeitelijkeVereniging(VCode vCode)
        => [EventFactory.SubtypeWerdVerfijndNaarFeitelijkeVereniging(vCode)];

    public IEvent[] ZetSubtypeNaarNietBepaald(VCode vCode)
        => [];

    public IEvent[] VerFijnNaarSubvereniging(VCode vCode, SubverenigingVanDto subverenigingVan)
    {
        VCode.ValidateVCode(subverenigingVan.AndereVereniging ?? string.Empty);

        return
        [
            EventFactory.VerenigingssubtypeWerdVerfijndNaarSubvereniging(
                vCode,
                subverenigingVan.AndereVereniging!,
                subverenigingVan.AndereVerenigingNaam!,
                subverenigingVan.Identificatie ?? string.Empty,
                subverenigingVan.Beschrijving ?? string.Empty)
        ];
    }
    public bool IsSubverenigingVan(VCode lidmaatschapAndereVereniging)
        => false;
}
