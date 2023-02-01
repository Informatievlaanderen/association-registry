namespace AssociationRegistry.Vertegenwoordigers;

using INSZ;

public class Vertegenwoordiger
{
    public Insz Insz { get; }
    public bool PrimairContactpersoon { get; }
    public string? Roepnaam { get; }
    public string? Rol { get; }
    public string Voornaam { get; }
    public string Achternaam { get; }

    public static Vertegenwoordiger Create(Insz insz,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        string voornaam,
        string achternaam)
        => new(insz, primairContactpersoon, roepnaam, rol, voornaam, achternaam);


    private Vertegenwoordiger(Insz insz,
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
