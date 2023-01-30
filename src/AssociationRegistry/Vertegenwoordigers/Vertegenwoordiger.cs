namespace AssociationRegistry.Vertegenwoordigers;

public class Vertegenwoordiger
{
    public string Rijksregisternummer { get; }
    public bool PrimairContactpersoon { get; }
    public string? Roepnaam { get; }
    public string? Rol { get; }
    public string Voornaam { get; }
    public string Achternaam { get; }

    public static Vertegenwoordiger Create(string rijksregisternummer,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        string voornaam,
        string achternaam)
        => new(rijksregisternummer, primairContactpersoon, roepnaam, rol, voornaam, achternaam);


    private Vertegenwoordiger(string rijksregisternummer,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        string voornaam,
        string achternaam)
    {
        Rijksregisternummer = rijksregisternummer;
        PrimairContactpersoon = primairContactpersoon;
        Roepnaam = roepnaam;
        Rol = rol;
        Voornaam = voornaam;
        Achternaam = achternaam;
    }
}
