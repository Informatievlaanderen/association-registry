namespace AssociationRegistry.Vereniging;

public class RelatieType
{
    public static readonly RelatieType IsAfdelingVan = new("Is afdeling van", "Heeft als afdeling");

    public static readonly RelatieType[] All = { IsAfdelingVan };

    public string Beschrijving { get; }
    public string InverseBeschrijving { get; }

    public RelatieType(string beschrijving, string inverseBeschrijving)
    {
        Beschrijving = beschrijving;
        InverseBeschrijving = inverseBeschrijving;
    }
}
