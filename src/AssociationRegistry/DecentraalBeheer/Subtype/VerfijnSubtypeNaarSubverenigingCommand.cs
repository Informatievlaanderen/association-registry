namespace AssociationRegistry.DecentraalBeheer.Subtype;

using Vereniging;

public record VerfijnSubtypeNaarSubverenigingCommand(
    VCode VCode,
    SubverenigingVan SubverenigingVan): IWijzigSubtypeCommand
{
}

public record SubverenigingVan(
    VCode? AndereVereniging,
    string? AndereVerenigingNaam,
    SubtypeIdentificatie? Identificatie,
    SubtypeBeschrijving? Beschrijving);
