namespace AssociationRegistry.Admin.Schema.Detail;

public record Vertegenwoordiger : IHasBron
{
    public JsonLdMetadata JsonLdMetadata { get; set; }
    public int VertegenwoordigerId { get; init; }
    public string Insz { get; set; } = null!;
    public string Voornaam { get; init; } = null!;
    public string Achternaam { get; init; } = null!;
    public string? Roepnaam { get; init; }
    public string? Rol { get; init; }
    public bool IsPrimair { get; init; }
    public string Email { get; init; } = null!;
    public string Telefoon { get; init; } = null!;
    public string Mobiel { get; init; } = null!;
    public string SocialMedia { get; init; } = null!;

    public int Identity
        => VertegenwoordigerId;

    public string Bron { get; set; } = null!;
    public VertegenwoordigerContactgegevens VertegenwoordigerContactgegevens { get; set; }
}
