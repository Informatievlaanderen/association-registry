namespace AssociationRegistry.Vereniging;

using DecentraalBeheer.Subtype;
using Events;

public interface IVerenigingssubtype
{

    IVerenigingssubtypeCode Code { get; }
    IVerenigingssubtype Apply(SubverenigingRelatieWerdGewijzigd @event);
    IVerenigingssubtype Apply(SubverenigingDetailsWerdenGewijzigd @event);
    IEvent[] VerFijnNaarFeitelijkeVereniging(VCode vCode);
    IEvent[] ZetSubtypeNaarNietBepaald(VCode vCode);
    IEvent[] VerFijnNaarSubvereniging(VCode vCode, VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan subverenigingVan);
    bool IsSubverenigingVan(VCode lidmaatschapAndereVereniging);
}
