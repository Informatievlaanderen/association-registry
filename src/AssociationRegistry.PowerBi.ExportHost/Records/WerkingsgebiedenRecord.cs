namespace AssociationRegistry.PowerBi.ExportHost.Records;

using CsvHelper.Configuration.Attributes;

public record WerkingsgebiedenRecord(
    [property: Name("code"), Index(0)] string Code,
    [property: Name("naam"), Index(1)] string Naam,
    [property: Name("vcode"), Index(2)] string VCode);
