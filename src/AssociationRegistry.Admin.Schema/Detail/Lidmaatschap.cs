namespace AssociationRegistry.Admin.Schema.Detail;

public record Lidmaatschap(
    JsonLdMetadata JsonLdMetadata,
    int LidmaatschapId,
    string AndereVereniging,
    DateOnly? Van,
    DateOnly? Tot,
    string Identificatie,
    string Beschrijving);
