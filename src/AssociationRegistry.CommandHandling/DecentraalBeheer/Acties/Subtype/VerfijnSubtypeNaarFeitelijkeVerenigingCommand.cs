namespace AssociationRegistry.DecentraalBeheer.Acties.Subtype;

using Vereniging;

public record VerfijnSubtypeNaarFeitelijkeVerenigingCommand(VCode VCode): IWijzigSubtypeCommand
{ }
