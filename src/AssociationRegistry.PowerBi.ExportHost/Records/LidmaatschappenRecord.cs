namespace AssociationRegistry.PowerBi.ExportHost.Records;

using CsvHelper.Configuration.Attributes;

record LidmaatschappenRecord(
    [property: Name("lidmaatschapId"), Index(0)] int LidmaatschapId,
    [property: Name("andereVereniging"), Index(1)] string AndereVereniging,
    [property: Name("van"), Index(2)] string Van,
    [property: Name("tot"), Index(3)] string Tot,
    [property: Name("identificatie"), Index(4)] string Identificatie,
    [property: Name("beschrijving"), Index(5)] string Beschrijving,
    [property: Name("vCode"), Index(6)] string VCode);
