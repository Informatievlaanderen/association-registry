namespace AssociationRegistry.Vereniging;

using Exceptions;

public class Werkingsgebied
{
    static Werkingsgebied()
    {
        All = NutsLauReader.Read().Select(x => new Werkingsgebied($"{x.Nuts}{x.Lau}", x.Gemeente)).ToArray();
    }

    private Werkingsgebied(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    public string Naam { get; }
    public string Code { get; }

    public static Werkingsgebied Create(string code)
    {
        var value = All.SingleOrDefault(p => string.Equals(p.Code, code, StringComparison.InvariantCultureIgnoreCase));

        return value ?? throw new WerkingsgebiedCodeIsNietGekend(code);
    }

    public static Werkingsgebied[] All { get; private set; }
}
