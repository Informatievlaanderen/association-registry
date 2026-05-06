namespace AssociationRegistry.Scheduled.Host.PowerBi.Records;

using CsvHelper.Configuration.Attributes;

record ErkenningenRecord(
    [property: Name("erkenningId"), Index(0)] int ErkenningId,
    [property: Name("gegevensInitiatorOvoCode"), Index(1)] string GegevensInitiatorOvoCode,
    [property: Name("gegevensInitiatorNaam"), Index(2)] string GegevensInitiatorNaam,
    [property: Name("ipdcProductNaam"), Index(3)] string IpdcProductNaam,
    [property: Name("ipdcProductNummer"), Index(4)] string IpdcProductNummer,
    [property: Name("startdatum"), Index(5)] string Startdatum,
    [property: Name("einddatum"), Index(6)] string einddatum,
    [property: Name("hernieuwingsdatum"), Index(7)] string Hernieuwingsdatum,
    [property: Name("hernieuwingsUrl"), Index(8)] string HernieuwingsUrl,
    [property: Name("redenSchorsing"), Index(9)] string RedenSchorsing,
    [property: Name("status"), Index(10)] string Status,
    [property: Name("vCode"), Index(11)] string VCode
);
