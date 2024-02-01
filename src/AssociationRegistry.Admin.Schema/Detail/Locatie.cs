namespace AssociationRegistry.Admin.Schema.Detail;

public record Locatie : IHasBron
{
    public JsonLdMetadata JsonLdMetadata { get; set; }
    public int LocatieId { get; set; }
    public LocatieType Locatietype { get; set; } = null!;
    public bool IsPrimair { get; set; }
    public string Adresvoorstelling { get; init; } = null!;
    public Adres? Adres { get; set; }
    public string? Naam { get; set; }
    public AdresId? AdresId { get; set; }
    public string Bron { get; set; } = null!;
}
