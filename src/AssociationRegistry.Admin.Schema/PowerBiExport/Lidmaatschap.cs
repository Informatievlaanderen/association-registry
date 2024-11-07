namespace AssociationRegistry.Admin.Schema.PowerBiExport;

public record Lidmaatschap(
    int LidmaatschapId,
    string AndereVereniging,
    string Van,
    string Tot,
    string Identificatie,
    string Beschrijving);
