namespace AssociationRegistry.Admin.Schema.Detail;

public record Lidmaatschap(
    JsonLdMetadata JsonLdMetadata,
    int LidmaatschapId,
    AndereVereniging AndereVereniging,
    DateOnly? Van,
    DateOnly? Tot,
    string Identificatie,
    string Beschrijving);

public record AndereVereniging(string VCode, string Naam);
