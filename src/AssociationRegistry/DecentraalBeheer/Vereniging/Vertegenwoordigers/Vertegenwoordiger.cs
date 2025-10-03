namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Emails;
using Magda.Kbo;
using SocialMedias;
using TelefoonNummers;

public record Vertegenwoordiger
{
    private Vertegenwoordiger(
        Insz insz,
        bool isPrimair,
        string? roepnaam,
        string? rol,
        Voornaam voornaam,
        Achternaam achternaam,
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
    public Voornaam Voornaam { get; }
    public Achternaam Achternaam { get; }
    public Email Email { get; }
    public TelefoonNummer Telefoon { get; }
    public TelefoonNummer Mobiel { get; }
    public SocialMedia SocialMedia { get; }

    public static Vertegenwoordiger Create(
        Insz insz,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        Voornaam voornaam,
        Achternaam achternaam,
        Email email,
        TelefoonNummer telefoonNummer,
        TelefoonNummer mobiel,
        SocialMedia socialMedia)
        => new(insz, primairContactpersoon, roepnaam, rol, voornaam, achternaam, email, telefoonNummer, mobiel, socialMedia);

    public static Vertegenwoordiger Hydrate(
        int vertegenwoordigerId,
        Insz insz,
        string? rol,
        string? roepnaam,
        Voornaam voornaam,
        Achternaam achternaam,
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

    public bool WouldBeEquivalent(
        string? rol,
        string? roepnaam,
        Email? email,
        TelefoonNummer? telefoonNummer,
        TelefoonNummer? mobiel,
        SocialMedia? socialMedia,
        bool? isPrimair,
        out Vertegenwoordiger updatedVertegenwoordiger)
    {
        updatedVertegenwoordiger = CopyWithValuesIfNotNull(rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair);

        return this == updatedVertegenwoordiger;
    }

    public bool WouldBeEquivalent(Vertegenwoordiger vertegenwoordiger)
        => this == vertegenwoordiger with { VertegenwoordigerId = VertegenwoordigerId };

    private Vertegenwoordiger CopyWithValuesIfNotNull(
        string? rol,
        string? roepnaam,
        Email? email,
        TelefoonNummer? telefoonNummer,
        TelefoonNummer? mobiel,
        SocialMedia? socialMedia,
        bool? isPrimair)
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

    public static Vertegenwoordiger CreateFromKbo(VertegenwoordigerVolgensKbo vertegenwoordigerVolgensKbo)
        => Create(
            Insz.Create(vertegenwoordigerVolgensKbo.Insz),
            false,
            string.Empty,
            string.Empty,
            Voornaam.Hydrate(vertegenwoordigerVolgensKbo.Voornaam),
            Achternaam.Hydrate(vertegenwoordigerVolgensKbo.Achternaam),
            Email.Leeg,
            TelefoonNummer.Leeg,
            TelefoonNummer.Leeg,
            SocialMedia.Leeg);
}
