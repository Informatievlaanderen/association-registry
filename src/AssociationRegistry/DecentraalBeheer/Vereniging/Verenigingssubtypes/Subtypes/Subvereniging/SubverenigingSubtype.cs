namespace AssociationRegistry.DecentraalBeheer.Vereniging.Subtypes.Subvereniging;

using Events;
using Events.Factories;
using Framework;
using Exceptions;

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

    public IEvent[] VerFijnNaarSubvereniging(VCode vCode, SubverenigingVanDto subverenigingVan)
    {
        Throw<WijzigSubverenigingMoetMinstensEenVeldTeWijzigenHebben>.If(
            TeWijzigenSubverenigingHeeftGeenVeldenTeWijzigen(subverenigingVan));

        return SubverenigingVan.Wijzig(vCode, subverenigingVan);
    }

    private bool TeWijzigenSubverenigingHeeftGeenVeldenTeWijzigen(SubverenigingVanDto subverenigingVan)
        => subverenigingVan.AndereVereniging is null && subverenigingVan.Identificatie is null && subverenigingVan.Beschrijving is null;
    public bool IsSubverenigingVan(VCode lidmaatschapAndereVereniging)
        => lidmaatschapAndereVereniging == SubverenigingVan.AndereVereniging;
}
