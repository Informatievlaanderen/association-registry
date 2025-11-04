namespace AssociationRegistry.Persoonsgegevens;

using Marten.Schema;

public class VertegenwoordigerPersoonsgegevens
{
    [Identity]
    public Guid RefId { get; set; }
    public string VCode { get; set; }
    public int VertegenwoordigerId { get; set; }
    public string Insz { get; init; }
    public bool IsPrimair { get; init; }
    public string? Roepnaam { get; }
    public string? Rol { get; }
    public string Voornaam { get; }
    public string Achternaam { get; }
    public string Email { get; }
    public string Telefoon { get; }
    public string Mobiel { get; }
    public string SocialMedia { get; }
}
