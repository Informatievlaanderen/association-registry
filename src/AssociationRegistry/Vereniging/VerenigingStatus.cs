namespace AssociationRegistry.Vereniging;

public class VerenigingStatus
{
    public static readonly VerenigingStatus Actief = new("Actief");
    public static readonly VerenigingStatus Gestopt = new("Gestopt");
    public static readonly VerenigingStatus Dubbel = new("Dubbel");

    public string Naam { get; }

    public VerenigingStatus(string naam)
    {
        Naam = naam;
    }

    public static readonly VerenigingStatus[] All =
    {
        Actief,
        Gestopt,
        Dubbel
    };

    public static VerenigingStatus Parse(string naam)
        => All.Single(t => t.Naam == naam);
}
