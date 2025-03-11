namespace AssociationRegistry.DecentraalBeheer.Subtype;

using Vereniging;

public record ZetSubtypeTerugNaarNietBepaaldCommand(VCode VCode) : IWijzigSubtypeCommand
{ }
