namespace AssociationRegistry.PowerBi.ExportHost.Records;

using CsvHelper.Configuration.Attributes;

public record BankrekeningnummerRecord(
    [property: Name("bankrekeningnummerId"), Index(0)] int BankrekeningnummerId,
    [property: Name("iban"), Index(1)] string Iban,
    [property: Name("gebruiktVoor"), Index(2)] string GebruiktVoor,
    [property: Name("titularis"), Index(3)] string Titularis,
    [property: Name("vCode"), Index(4)] string VCode
    );
