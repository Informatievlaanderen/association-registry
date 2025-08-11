namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Subtypes.Subvereniging;
using Events;

public interface IVerenigingssubtype
{

    IVerenigingssubtypeCode Code { get; }
    IVerenigingssubtype Apply(SubverenigingRelatieWerdGewijzigd @event);
    IVerenigingssubtype Apply(SubverenigingDetailsWerdenGewijzigd @event);
    IEvent[] VerFijnNaarFeitelijkeVereniging(VCode vCode);
    IEvent[] ZetSubtypeNaarNietBepaald(VCode vCode);
    IEvent[] VerFijnNaarSubvereniging(VCode vCode, SubverenigingVanDto subverenigingVan);
    bool IsSubverenigingVan(VCode lidmaatschapAndereVereniging);
}
