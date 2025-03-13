namespace AssociationRegistry.DecentraalBeheer.Subtype;

using Vereniging;

public record VerfijnSubtypeNaarSubverenigingCommand(
    VCode VCode,
    VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan SubverenigingVan): IWijzigSubtypeCommand
{
    public static class Data
    {
        public record SubverenigingVan(
            VCode? AndereVereniging,
            string? AndereVerenigingNaam,
            SubtypeIdentificatie? Identificatie,
            SubtypeBeschrijving? Beschrijving);
    }
}


