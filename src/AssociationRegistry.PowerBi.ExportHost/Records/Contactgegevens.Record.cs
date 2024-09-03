namespace AssociationRegistry.PowerBi.ExportHost.Records;

using CsvHelper.Configuration.Attributes;

public record ContactgegevensRecord(
    [property: Name("beschrijving"), Index(0)] string Beschrijving,
    [property: Name("bron"), Index(1)] string Bron,
    [property: Name("contactgegevenId"), Index(2)] int ContactgegevenId,
    [property: Name("contactgegevenType"), Index(3)] string ContactgegevenType,
    [property: Name("isPrimair"), Index(4)] bool IsPrimair,
    [property: Name("vCode"), Index(5)] string VCode,
    [property: Name("waarde"), Index(6)] string Waarde);
