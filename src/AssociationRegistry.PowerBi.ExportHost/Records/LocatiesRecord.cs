namespace AssociationRegistry.PowerBi.ExportHost.Records;

using CsvHelper.Configuration.Attributes;

record LocatiesRecord(
    [property: Name("adresId.broncode"), Index(0)] string AdresIdBroncode,
    [property: Name("adresId.bronwaarde"), Index(1)] string AdresIdBronwaarde,
    [property: Name("adresvoorstelling"), Index(2)] string Adresvoorstelling,
    [property: Name("bron"), Index(3)] string Bron,
    [property: Name("busnummer"), Index(4)] string Busnummer,
    [property: Name("gemeente"), Index(5)] string Gemeente,
    [property: Name("huisnummer"), Index(6)] string Huisnummer,
    [property: Name("isPrimair"), Index(7)] bool IsPrimair,
    [property: Name("land"), Index(8)] string Land,
    [property: Name("locatieId"), Index(9)] int LocatieId,
    [property: Name("locatieType"), Index(10)] string LocatieType,
    [property: Name("naam"), Index(11)] string Naam,
    [property: Name("postcode"), Index(12)] string PostCode,
    [property: Name("straatnaam"), Index(13)] string Straatnaam,
    [property: Name("vCode"), Index(14)] string VCode);
