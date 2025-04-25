namespace AssociationRegistry.Vereniging;

using DecentraalBeheer.Subtype;
using Events;

public interface IHasVerenigingssubtypeCodeAndNaam
{
    string Code { get; init; }
    string Naam { get; init; }
}

public interface IVerenigingssubtype
{
    string Code { get; }
    string Naam { get; }
    IVerenigingssubtype Apply(SubverenigingRelatieWerdGewijzigd @event);
    IVerenigingssubtype Apply(SubverenigingDetailsWerdenGewijzigd @event);
    IEvent[] VerFijnNaarFeitelijkeVereniging(VCode vCode);
    IEvent[] ZetSubtypeNaarNietBepaald(VCode vCode);
    IEvent[] VerFijnNaarSubvereniging(VCode vCode, VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan subverenigingVan);
    bool IsSubverenigingVan(VCode lidmaatschapAndereVereniging);
}
