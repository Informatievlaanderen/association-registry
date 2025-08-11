namespace AssociationRegistry.DecentraalBeheer.Vereniging.Subtypes.NoSubtype;

using Subvereniging;
using Events;

public record NoSubtype : IVerenigingssubtype
{
    public IVerenigingssubtypeCode Code => throw new InvalidOperationException("This subtype is not supposed to be used.");

    public IVerenigingssubtype Apply(SubverenigingRelatieWerdGewijzigd @event)
        => throw new InvalidOperationException("This subtype is not supposed to be used.");

    public IVerenigingssubtype Apply(SubverenigingDetailsWerdenGewijzigd @event)
        => throw new InvalidOperationException("This subtype is not supposed to be used.");

    public IEvent[] VerFijnNaarFeitelijkeVereniging(VCode vCode)
        => throw new InvalidOperationException("This subtype is not supposed to be used.");

    public IEvent[] ZetSubtypeNaarNietBepaald(VCode vCode)
        => throw new InvalidOperationException("This subtype is not supposed to be used.");


    public IEvent[] VerFijnNaarSubvereniging(VCode vCode, SubverenigingVanDto subverenigingVan)
        => throw new InvalidOperationException("This subtype is not supposed to be used.");

    public bool IsSubverenigingVan(VCode lidmaatschapAndereVereniging)
        => false;
}
