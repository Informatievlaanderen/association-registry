namespace AssociationRegistry.PowerBi.ExportHost.Records;

using CsvHelper.Configuration.Attributes;

public record HistoriekRecord(
    [property: Name("datum"), Index(0)] string Datum,
    [property: Name("gebeurtenis"), Index(1)] string Gebeurtenis,
    [property: Name("initiator"), Index(2)] string Initiator,
    [property: Name("tijdstip"), Index(3)] string Tijdstip,
    [property: Name("vCode"), Index(4)] string VCode,
    [property: Name("sequence"), Index(5)] long Sequence);
