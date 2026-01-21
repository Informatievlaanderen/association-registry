namespace AssociationRegistry.Admin.Schema.Persoonsgegevens;

using Marten.Schema;

public class BankrekeningnummerPersoonsgegevensDocument
{
    [Identity]
    public Guid RefId { get; set; }
    public string VCode { get; set; }
    public int BankrekeningnummerId { get; set; }
    public string? Iban { get; init; }
    public string? Titularis { get; set; }
}
