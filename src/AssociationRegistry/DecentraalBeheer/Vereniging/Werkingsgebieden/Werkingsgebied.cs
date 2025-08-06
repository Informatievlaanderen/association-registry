namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public record Werkingsgebied
{
    public Werkingsgebied(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    public static Werkingsgebied NietVanToepassing => new("NVT", "Niet van toepassing");

    public string Naam { get; }
    public string Code { get; }

    public static Werkingsgebied Hydrate(string code, string naam)
        => new Werkingsgebied(code, naam);
    public static Werkingsgebied Hydrate(string nuts, string lau, string naam)
        => new Werkingsgebied($"{nuts}{lau}", naam);
}
