namespace AssociationRegistry.PowerBi.ExportHost.Records.DubbelDetectie;

using CsvHelper.Configuration.Attributes;

record DubbelDetectieRecord(
    [property: Name("bevestigingstokenKey"), Index(0)] string BevestigingstokenKey,
    [property: Name("bevestigingstoken"), Index(1)] string Bevestigingstoken,
    [property: Name("naam"), Index(2)] string Naam,
    [property: Name("postcodes"), Index(3)] string Postcodes,
    [property: Name("gemeentes"), Index(4)] string Gemeentes,
    [property: Name("gedetecteerdeDubbels"), Index(5)] string GedetecteerdeDubbels,
    [property: Name("tijdstip"), Index(6)] string Tijdstip,
    [property: Name("initiator"), Index(7)] string Initiator,
    [property: Name("correlationId"), Index(8)] string CorrelationId
    );

