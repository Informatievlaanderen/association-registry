namespace AssociationRegistry.Admin.Schema.Detail;

public record Contactgegeven : IHasBron
{
    public JsonLdMetadata JsonLdMetadata { get; set; }
    public int ContactgegevenId { get; init; }
    public string Contactgegeventype { get; init; } = null!;
    public string Waarde { get; init; } = null!;
    public string? Beschrijving { get; init; }
    public bool IsPrimair { get; init; }
    public string Bron { get; set; } = null!;
}