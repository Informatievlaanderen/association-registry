namespace AssociationRegistry.Vereniging;

using Emails;
using Magda;
using SocialMedias;
using TelefoonNummers;

public record Vertegenwoordiger
{
    private Vertegenwoordiger(
        Insz insz,
        bool isPrimair,
        string? roepnaam,
        string? rol,
        string voornaam,
        string achternaam,
        Email email,
        TelefoonNummer telefoon,
        TelefoonNummer mobiel,
        SocialMedia socialMedia)
    {
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

    public int VertegenwoordigerId { get; set; }
    public Insz Insz { get; init; }
    public bool IsPrimair { get; init; }
    public string? Roepnaam { get; }
    public string? Rol { get; }
    public string Voornaam { get; }
    public string Achternaam { get; }
    public Email Email { get; }
    public TelefoonNummer Telefoon { get; }
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
            vertegenwoordiger.IsPrimair,
            vertegenwoordiger.Roepnaam,
            vertegenwoordiger.Rol,
            persoon.Voornaam,
            persoon.Achternaam,
            vertegenwoordiger.Email,
            vertegenwoordiger.Telefoon,
            vertegenwoordiger.Mobiel,
            vertegenwoordiger.SocialMedia);

    public static Vertegenwoordiger Hydrate(
        int vertegenwoordigerId,
        Insz insz,
        string? rol,
        string? roepnaam,
        string voornaam,
        string achternaam,
        bool isPrimair,
        Email email,
        TelefoonNummer telefoon,
        TelefoonNummer mobiel,
        SocialMedia socialMedia)
        => new(
            insz,
            isPrimair,
            roepnaam,
            rol,
            voornaam,
            achternaam,
            email,
            telefoon,
            mobiel,
            socialMedia)
        {
            VertegenwoordigerId = vertegenwoordigerId,
        };

    public bool WouldBeEquivalent(string? rol, string? roepnaam, Email? email, TelefoonNummer? telefoonNummer, TelefoonNummer? mobiel, SocialMedia? socialMedia, bool? isPrimair, out Vertegenwoordiger updatedVertegenwoordiger)
    {
        updatedVertegenwoordiger = CopyWithValuesIfNotNull(rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair);
        return this == updatedVertegenwoordiger;
    }

    private Vertegenwoordiger CopyWithValuesIfNotNull(string? rol, string? roepnaam, Email? email, TelefoonNummer? telefoonNummer, TelefoonNummer? mobiel, SocialMedia? socialMedia, bool? isPrimair)
        => Create(
                Insz,
                isPrimair ?? IsPrimair,
                roepnaam ?? Roepnaam,
                rol ?? Rol,
                Voornaam,
                Achternaam,
                email ?? Email,
                telefoonNummer ?? Telefoon,
                mobiel ?? Mobiel,
                socialMedia ?? SocialMedia) with
            {
                VertegenwoordigerId = VertegenwoordigerId,
            };
}
