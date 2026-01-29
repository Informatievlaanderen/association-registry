namespace AssociationRegistry.Admin.Schema.Detail;

public record Bankrekeningnummer
{
    public JsonLdMetadata JsonLdMetadata { get; set; }

    public int BankrekeningnummerId { get; set; }
    public string Iban { get; set; } = null!;
    public string Doel { get; set; } = null!;
    public string Titularis { get; set; } = null!;
    public bool IsGevalideerd { get; set; }
}
