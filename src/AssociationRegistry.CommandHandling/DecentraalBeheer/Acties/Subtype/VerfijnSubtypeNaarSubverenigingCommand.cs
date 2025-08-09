namespace AssociationRegistry.DecentraalBeheer.Acties.Subtype;

using Vereniging;
using Vereniging.Subtypes.Subvereniging;

public record VerfijnSubtypeNaarSubverenigingCommand(
    VCode VCode,
    VerfijnSubtypeNaarSubverenigingCommand.Data.SubverenigingVan SubverenigingVan): IWijzigSubtypeCommand
{
    public static class Data
    {
        public record SubverenigingVan(
            VCode? AndereVereniging,
            SubverenigingIdentificatie? Identificatie,
            SubverenigingBeschrijving? Beschrijving)
        {
            public string AndereVerenigingNaam { get; set; } = string.Empty;
        };
    }
}


