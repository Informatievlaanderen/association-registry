namespace AssociationRegistry.Vereniging;

public class Relatietype
{
    public static readonly Relatietype IsAfdelingVan = new(beschrijving: "Is afdeling van", inverseBeschrijving: "Heeft als afdeling");
    public static readonly Relatietype[] All = { IsAfdelingVan };

    public Relatietype(string beschrijving, string inverseBeschrijving)
    {
        Beschrijving = beschrijving;
        InverseBeschrijving = inverseBeschrijving;
    }

    public string Beschrijving { get; }
    public string InverseBeschrijving { get; }
}
