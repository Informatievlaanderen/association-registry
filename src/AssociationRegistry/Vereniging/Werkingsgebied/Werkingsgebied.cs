namespace AssociationRegistry.Vereniging.Werkingsgebied;

public class Werkingsgebied
{
    private Werkingsgebied(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    public string Naam { get; }
    public string Code { get; }

    public static Werkingsgebied Parse(string s)
    {
        return new Werkingsgebied("BE", "Belgie");
    }
}
