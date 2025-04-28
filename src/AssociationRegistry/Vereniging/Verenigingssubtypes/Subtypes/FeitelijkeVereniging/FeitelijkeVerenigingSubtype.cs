namespace AssociationRegistry.Vereniging;

using DecentraalBeheer.Subtype;
using EventFactories;
using Events;

public record FeitelijkeVerenigingSubtype : IVerenigingssubtype
{
    public IVerenigingssubtypeCode Code => VerenigingssubtypeCodering.FeitelijkeVereniging;

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