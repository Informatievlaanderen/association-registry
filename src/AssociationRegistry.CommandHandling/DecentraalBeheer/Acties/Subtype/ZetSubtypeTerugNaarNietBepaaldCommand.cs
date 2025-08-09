namespace AssociationRegistry.DecentraalBeheer.Acties.Subtype;

using Vereniging;

public record ZetSubtypeTerugNaarNietBepaaldCommand(VCode VCode) : IWijzigSubtypeCommand
{ }
