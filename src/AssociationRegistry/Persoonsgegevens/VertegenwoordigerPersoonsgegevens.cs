namespace AssociationRegistry.Persoonsgegevens;

using DecentraalBeheer.Vereniging;

public record VertegenwoordigerPersoonsgegevens
{
    public Guid RefId { get; }
    public VCode VCode { get; }
    public int VertegenwoordigerId { get; }
    public Insz Insz { get; }
    public bool IsPrimair { get; }
    public string? Roepnaam { get; }
    public string? Rol { get; }
    public Voornaam Voornaam { get; }
    public Achternaam Achternaam { get; }
    public string Email { get; }
    public string Telefoon { get; }
    public string Mobiel { get; }
    public string SocialMedia { get; }

    public VertegenwoordigerPersoonsgegevens(
        Guid refId,
        VCode VCode,
        int vertegenwoordigerId,
        Insz insz,
        bool isPrimair,
        string? roepnaam,
        string? rol,
        Voornaam voornaam,
        Achternaam achternaam,
        string email,
        string telefoon,
        string mobiel,
        string socialMedia)
    {
        RefId = refId;
        this.VCode = VCode;
        VertegenwoordigerId = vertegenwoordigerId;
        Insz = insz;
        IsPrimair = isPrimair;
        Roepnaam = roepnaam;
        Rol = rol;
        Voornaam = voornaam;
        Achternaam = achternaam;
        Email = email;
        Telefoon = telefoon;
        Mobiel = mobiel;
        SocialMedia = socialMedia;
    }
}
