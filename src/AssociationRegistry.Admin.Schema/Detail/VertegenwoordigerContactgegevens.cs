namespace AssociationRegistry.Admin.Schema.Detail;

public record VertegenwoordigerContactgegevens
{
    public JsonLdMetadata JsonLdMetadata { get; set; }
    public bool IsPrimair { get; init; }
    public string Email { get; init; } = null!;
    public string Telefoon { get; init; } = null!;
    public string Mobiel { get; init; } = null!;
    public string SocialMedia { get; init; } = null!;
}
