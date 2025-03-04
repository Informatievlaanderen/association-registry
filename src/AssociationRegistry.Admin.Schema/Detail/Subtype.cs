namespace AssociationRegistry.Admin.Schema.Detail;

public record Subtype
{
    public JsonLdMetadata JsonLdMetadata { get; init; }
    public string Code { get; init; } = null!;
    public string Naam { get; init; } = null!;
}
