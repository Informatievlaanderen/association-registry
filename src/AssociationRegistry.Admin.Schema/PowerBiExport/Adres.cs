namespace AssociationRegistry.Admin.Schema.PowerBiExport;

public class Adres
{
    public string Straatnaam { get; init; } = null!;
    public string Huisnummer { get; init; } = null!;
    public string? Busnummer { get; init; }
    public string Postcode { get; init; } = null!;
    public string Gemeente { get; init; } = null!;
    public string Land { get; init; } = null!;
}
