namespace AssociationRegistry.Vereniging;

using Emails;
using Magda;
using SocialMedias;
using TelefoonNummers;

public record Vertegenwoordiger
{
    private Vertegenwoordiger(
        Insz insz,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        string voornaam,
        string achternaam,
        Email email,
        TelefoonNummer telefoonNummer,
        TelefoonNummer mobiel,
        SocialMedia socialMedia)
    {
        Insz = insz;
        PrimairContactpersoon = primairContactpersoon;
        Roepnaam = roepnaam;
        Rol = rol;
        Voornaam = voornaam;
        Achternaam = achternaam;
        Email = email;
        TelefoonNummer = telefoonNummer;
        Mobiel = mobiel;
        SocialMedia = socialMedia;
    }

    public int VertegenwoordigerId { get; set; }
    public Insz Insz { get; init; }
    public bool PrimairContactpersoon { get; init; }
    public string? Roepnaam { get; }
    public string? Rol { get; }
    public string Voornaam { get; }
    public string Achternaam { get; }
    public Email Email { get; }
    public TelefoonNummer TelefoonNummer { get; }
    public TelefoonNummer Mobiel { get; }
    public SocialMedia SocialMedia { get; }

    public static Vertegenwoordiger Create(
        Insz insz,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        string voornaam,
        string achternaam,
        Email email,
        TelefoonNummer telefoonNummer,
        TelefoonNummer mobiel,
        SocialMedia socialMedia)
        => new(insz, primairContactpersoon, roepnaam, rol, voornaam, achternaam, email, telefoonNummer, mobiel, socialMedia);

    public static Vertegenwoordiger Create(
        Insz insz,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        Email email,
        TelefoonNummer telefoonNummer,
        TelefoonNummer mobiel,
        SocialMedia socialMedia)
        => new(insz, primairContactpersoon, roepnaam, rol, string.Empty, string.Empty, email, telefoonNummer, mobiel, socialMedia);

    internal static Vertegenwoordiger Enrich(Vertegenwoordiger vertegenwoordiger, MagdaPersoon persoon)
        => new(
            vertegenwoordiger.Insz,
            vertegenwoordiger.PrimairContactpersoon,
            vertegenwoordiger.Roepnaam,
            vertegenwoordiger.Rol,
            persoon.Voornaam,
            persoon.Achternaam,
            vertegenwoordiger.Email,
            vertegenwoordiger.TelefoonNummer,
            vertegenwoordiger.Mobiel,
            vertegenwoordiger.SocialMedia);
}
