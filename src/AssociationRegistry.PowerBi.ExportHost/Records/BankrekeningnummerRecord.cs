namespace AssociationRegistry.PowerBi.ExportHost.Records;

using CsvHelper.Configuration.Attributes;

public record BankrekeningnummerRecord(
    [property: Name("bankrekeningnummerId"), Index(0)] int BankrekeningnummerId,
    [property: Name("doel"), Index(1)] string Doel,
    [property: Name("vCode"), Index(2)] string VCode,
    [property: Name("bevestigdDoor"), Index(3)] string BevestigdDoor,
    [property: Name("bron"), Index(4)] string Bron
    );
