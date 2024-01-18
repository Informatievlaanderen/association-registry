namespace AssociationRegistry.Admin.Schema.Detail;

public class Adres
{        
    public JsonLdMetadata JsonLdMetadata { get; set; } = null!;
    public string Straatnaam { get; init; } = null!;
    public string Huisnummer { get; init; } = null!;
    public string? Busnummer { get; init; }
    public string Postcode { get; init; } = null!;
    public string Gemeente { get; init; } = null!;
    public string Land { get; init; } = null!;
}