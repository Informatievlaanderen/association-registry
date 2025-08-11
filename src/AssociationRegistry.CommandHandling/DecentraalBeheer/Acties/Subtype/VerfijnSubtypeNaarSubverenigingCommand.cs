namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Subtypes.Subvereniging;

public record VerfijnSubtypeNaarSubverenigingCommand(
    VCode VCode,
    SubverenigingVanDto SubverenigingVan): IWijzigSubtypeCommand;


