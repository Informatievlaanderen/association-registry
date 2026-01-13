namespace AssociationRegistry.Admin.Schema.Detail;

public class Bankrekeningnummer
{
    public JsonLdMetadata JsonLdMetadata { get; set; }

    public int BankrekeningnummerId { get; set; }
    public string Iban { get; set; } = null!;
    public string GebruiktVoor { get; set; } = null!;
    public string Titularis { get; set; } = null!;
}
