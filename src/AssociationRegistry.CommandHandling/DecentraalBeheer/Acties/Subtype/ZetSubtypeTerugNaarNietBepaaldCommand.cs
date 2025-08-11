namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record ZetSubtypeTerugNaarNietBepaaldCommand(VCode VCode) : IWijzigSubtypeCommand
{ }
