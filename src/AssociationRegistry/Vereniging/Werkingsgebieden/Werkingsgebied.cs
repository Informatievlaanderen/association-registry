namespace AssociationRegistry.Vereniging;

using Exceptions;

public class Werkingsgebied
{
    static Werkingsgebied()
    {
        All = new[]
              {
                  new Werkingsgebied("BE21", "Provincie Antwerpen"),
                  new Werkingsgebied("BE22", "Provincie Limburg"),
                  new Werkingsgebied("BE23", "Provincie Oost-Vlaanderen"),
                  new Werkingsgebied("BE24", "Provincie Vlaams-Brabant"),
                  new Werkingsgebied("BE25", "Provincie West-Vlaanderen"),
              }
             .Union(NutsLauReader
                   .Read()
                   .Select(x => new Werkingsgebied($"{x.Nuts}{x.Lau}", x.Gemeente)))
             .ToArray();

        AllWithNVT = new[]
                     {
                         new Werkingsgebied("NVT", "Niet van toepassing"),
                     }
                    .Union(All)
                    .ToArray();
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
        var value = AllWithNVT.SingleOrDefault(p => string.Equals(p.Code, code, StringComparison.InvariantCultureIgnoreCase));

        return value ?? throw new WerkingsgebiedCodeIsNietGekend(code);
    }

    public static Werkingsgebied Hydrate(string code, string naam)
        => new Werkingsgebied(code, naam);

    public static Werkingsgebied[] AllWithNVT { get; }
    public static Werkingsgebied[] All { get; }
}
