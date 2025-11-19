namespace AssociationRegistry.Persoonsgegevens;

using DecentraalBeheer.Vereniging;

public record VertegenwoordigerPersoonsgegevens
{
    public Guid RefId { get; init; }
    public VCode VCode { get; init; }
    public int VertegenwoordigerId { get; init; }
    public Insz Insz { get; init; }
    public string? Roepnaam { get; init; }
    public string? Rol { get; init; }
    public Voornaam Voornaam { get; init; }
    public Achternaam Achternaam { get; init; }
    public string Email { get; init; }
    public string Telefoon { get; init; }
    public string Mobiel { get; init; }
    public string SocialMedia { get; init; }

    public VertegenwoordigerPersoonsgegevens(
        Guid refId,
        VCode VCode,
        int vertegenwoordigerId,
        Insz insz,
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
        Roepnaam = roepnaam;
        Rol = rol;
        Voornaam = voornaam;
        Achternaam = achternaam;
        Email = email;
        Telefoon = telefoon;
        Mobiel = mobiel;
        SocialMedia = socialMedia;
    }

    public static VertegenwoordigerPersoonsgegevens ToVertegenwoordiger(Guid refId, VCode VCode, Vertegenwoordiger vertegenwoordiger)
        => new(refId,
               VCode,
               vertegenwoordiger.VertegenwoordigerId,
               vertegenwoordiger.Insz,
               vertegenwoordiger.Roepnaam,
               vertegenwoordiger.Rol,
               vertegenwoordiger.Voornaam,
               vertegenwoordiger.Achternaam,
               vertegenwoordiger.Email.Waarde,
               vertegenwoordiger.Telefoon.Waarde,
               vertegenwoordiger.Mobiel.Waarde,
               vertegenwoordiger.SocialMedia.Waarde);
}
