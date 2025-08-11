namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record VerfijnSubtypeNaarFeitelijkeVerenigingCommand(VCode VCode): IWijzigSubtypeCommand
{ }
