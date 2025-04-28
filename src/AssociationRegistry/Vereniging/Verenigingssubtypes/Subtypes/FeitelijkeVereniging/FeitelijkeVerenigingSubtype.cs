namespace AssociationRegistry.Vereniging.Subtypes.FeitelijkeVereniging;

using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Events;

public record FeitelijkeVerenigingSubtype : IVerenigingssubtype
{
    public IVerenigingssubtypeCode Code => VerenigingssubtypeCode.FeitelijkeVereniging;

    public IVerenigingssubtype Apply(SubverenigingRelatieWerdGewijzigd @event)
        => this;

    public IVerenigingssubtype Apply(SubverenigingDetailsWerdenGewijzigd @event)
        => this;

    public IEvent[] VerFijnNaarFeitelijkeVereniging(VCode vCode)
        => [];
    public IEvent[] ZetSubtypeNaarNietBepaald(VCode vCode)
        => [EventFactory.SubtypeWerdTerugGezetNaarNietBepaald(vCode)];

    public IEvent[] VerFijnNaarSubvereniging(VCode vCode, VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan subverenigingVan)
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
