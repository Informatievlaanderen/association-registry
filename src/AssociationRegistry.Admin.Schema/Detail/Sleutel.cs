namespace AssociationRegistry.Admin.Schema.Detail;

public record Sleutel
{
    public JsonLdMetadata JsonLdMetadata { get; set; }
    public string Bron { get; init; } = null!;
    public string Waarde { get; init; } = null!;
    public GestructureerdeIdentificator GestructureerdeIdentificator { get; set; }
}