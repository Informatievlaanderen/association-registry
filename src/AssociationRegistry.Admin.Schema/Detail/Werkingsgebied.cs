namespace AssociationRegistry.Admin.Schema.Detail;

public record Werkingsgebied
{
    public JsonLdMetadata JsonLdMetadata { get; set; }
    public string Code { get; init; } = null!;
    public string Naam { get; init; } = null!;
}
