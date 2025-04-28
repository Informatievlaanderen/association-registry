namespace AssociationRegistry.Vereniging.Subtypes.Subvereniging;

using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging.Exceptions;

public record SubverenigingSubtype : IVerenigingssubtype
{
    public SubverenigingVan SubverenigingVan { get; }

    public IVerenigingssubtypeCode Code => VerenigingssubtypeCode.Subvereniging;

    public SubverenigingSubtype(SubverenigingVan subverenigingVan)
    {
        SubverenigingVan = subverenigingVan;
    }
    public IVerenigingssubtype Apply(SubverenigingRelatieWerdGewijzigd @event)
        => new SubverenigingSubtype(SubverenigingVan.Hydrate(@event));

    public IVerenigingssubtype Apply(SubverenigingDetailsWerdenGewijzigd @event)
        => new SubverenigingSubtype(SubverenigingVan.Hydrate(@event));

    public IEvent[] VerFijnNaarFeitelijkeVereniging(VCode vCode)
        => [EventFactory.SubtypeWerdVerfijndNaarFeitelijkeVereniging(vCode)];

    public IEvent[] ZetSubtypeNaarNietBepaald(VCode vCode)
        => [EventFactory.SubtypeWerdTerugGezetNaarNietBepaald(vCode)];

    public IEvent[] VerFijnNaarSubvereniging(VCode vCode, VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan subverenigingVan)
    {
        Throw<WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben>.If(
            TeWijzigenSubverenigingHeeftGeenVeldenTeWijzigen(subverenigingVan));

        return SubverenigingVan.Wijzig(vCode, subverenigingVan);
    }

    private bool TeWijzigenSubverenigingHeeftGeenVeldenTeWijzigen(VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan subverenigingVan)
        => subverenigingVan.AndereVereniging is null && subverenigingVan.Identificatie is null && subverenigingVan.Beschrijving is null;
    public bool IsSubverenigingVan(VCode lidmaatschapAndereVereniging)
        => lidmaatschapAndereVereniging == SubverenigingVan.AndereVereniging;
}
