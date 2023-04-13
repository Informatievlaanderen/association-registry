namespace AssociationRegistry.Vereniging;

using Magda;

public record Vertegenwoordiger
{
    public Insz Insz { get; }
    public bool PrimairContactpersoon { get; }
    public string? Roepnaam { get; }
    public string? Rol { get; }
    public string Voornaam { get; }
    public string Achternaam { get; }
    public Contactgegeven[] Contactgegevens { get; init; }

    public static Vertegenwoordiger Create(
        Insz insz,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        string voornaam,
        string achternaam,
        Contactgegevens contactgegevens)
        => new(insz, primairContactpersoon, roepnaam, rol, voornaam, achternaam, contactgegevens.ToArray());

    public static Vertegenwoordiger Create(
        Insz insz,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        Contactgegeven[] contactgegevens)
        => new(insz, primairContactpersoon, roepnaam, rol, string.Empty, string.Empty, contactgegevens);

    private Vertegenwoordiger(
        Insz insz,
        bool primairContactpersoon,
        string? roepnaam,
        string? rol,
        string voornaam,
        string achternaam,
        Contactgegeven[] contactgegevens)
    {
        Insz = insz;
        PrimairContactpersoon = primairContactpersoon;
        Roepnaam = roepnaam;
        Rol = rol;
        Voornaam = voornaam;
        Achternaam = achternaam;
        Contactgegevens = contactgegevens;
    }

    internal static Vertegenwoordiger Enrich(Vertegenwoordiger vertegenwoordiger, MagdaPersoon persoon)
        => new(
            vertegenwoordiger.Insz,
            vertegenwoordiger.PrimairContactpersoon,
            vertegenwoordiger.Roepnaam,
            vertegenwoordiger.Rol,
            persoon.Voornaam,
            persoon.Achternaam,
            vertegenwoordiger.Contactgegevens);
}
