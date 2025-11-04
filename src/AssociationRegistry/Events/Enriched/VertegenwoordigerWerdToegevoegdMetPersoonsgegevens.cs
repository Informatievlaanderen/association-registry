namespace AssociationRegistry.Events.Enriched;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record VertegenwoordigerPersoonsgegevens
{
    public bool IsVerlopen  { get; set; }

    public static VertegenwoordigerPersoonsgegevens Create(
        string insz,
        string? roepnaam,
        string? rol,
        string voornaam,
        string achternaam,
        string email,
        string telefoon,
        string mobiel,
        string socialMedia)
        => new(false,insz, roepnaam,  rol,  voornaam, achternaam,
               email, telefoon, mobiel, socialMedia);

    public static VertegenwoordigerPersoonsgegevens Verlopen
        => new(true);

    private VertegenwoordigerPersoonsgegevens(bool isVerlopen)
    {
        IsVerlopen = isVerlopen;
    }

    private VertegenwoordigerPersoonsgegevens(bool isVerlopen, string insz, string? roepnaam, string? rol, string voornaam, string achternaam, string email, string telefoon, string mobiel, string socialMedia)
    {
        IsVerlopen = isVerlopen;
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

    public string? Insz { get; private set; }
    public string? Roepnaam { get; private set; }
    public string? Rol { get; private set; }
    public string? Voornaam { get; private set; }
    public string? Achternaam { get; private set; }
    public string? Email { get; private set; }
    public string? Telefoon { get; private set; }
    public string? Mobiel { get; private set; }
    public string? SocialMedia { get; private set; }
}

public record VertegenwoordigerWerdToegevoegdMetPersoonsgegevens(
    int VertegenwoordigerId,
    bool IsPrimair,
    VertegenwoordigerPersoonsgegevens VertegenwoordigerPersoonsgegevens) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
};
