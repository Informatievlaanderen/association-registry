namespace AssociationRegistry.Persoonsgegevens;

using Marten.Schema;

public class VertegenwoordigerPersoonsgegevensDocument
{
    [Identity]
    public Guid RefId { get; set; }
    public string VCode { get; set; }
    public int VertegenwoordigerId { get; set; }
    public string Insz { get; init; }
    public bool IsPrimair { get; init; }
    public string? Roepnaam { get; set; }
    public string? Rol { get; set; }
    public string Voornaam { get; set; }
    public string Achternaam { get; set; }
    public string Email { get; set; }
    public string Telefoon { get; set; }
    public string Mobiel { get; set; }
    public string SocialMedia { get; set; }
}
