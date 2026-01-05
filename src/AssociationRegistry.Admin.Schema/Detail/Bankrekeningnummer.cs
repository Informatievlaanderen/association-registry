namespace AssociationRegistry.Admin.Schema.Detail;

public class Bankrekeningnummer
{
    public JsonLdMetadata JsonLdMetadata { get; set; }
    public string IBAN { get; set; } = null!;
    public string GebruiktVoor { get; set; } = null!;
    public string Titularis { get; set; } = null!;
}
