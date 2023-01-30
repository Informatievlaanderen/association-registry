namespace AssociationRegistry.Vertegenwoordigers;

public class Vertegenwoordiger
{
    public string Insz { get; }
    public bool PrimairContactpersoon { get; }
    public string? Roepnaam { get; }
    public string? Rol { get; }
    public string Voornaam { get; }
    public string Achternaam { get; }

    public static Vertegenwoordiger Create(string insz,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        string voornaam,
        string achternaam)
        => new(insz, primairContactpersoon, roepnaam, rol, voornaam, achternaam);


    private Vertegenwoordiger(string insz,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        string voornaam,
        string achternaam)
    {
        Insz = insz;
        PrimairContactpersoon = primairContactpersoon;
        Roepnaam = roepnaam;
        Rol = rol;
        Voornaam = voornaam;
        Achternaam = achternaam;
    }
}
