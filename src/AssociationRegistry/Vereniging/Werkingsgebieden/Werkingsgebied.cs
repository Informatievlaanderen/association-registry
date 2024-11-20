namespace AssociationRegistry.Vereniging;

using Exceptions;

public record Werkingsgebied
{
    static Werkingsgebied()
    {
        All = new[]
              {
                  new Werkingsgebied("BE10", "Brussels Hoofdstedelijk Gewest"),
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
                         NietVanToepassing,
                     }
                    .Union(All)
                    .ToArray();
    }

    private Werkingsgebied(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    public static Werkingsgebied NietVanToepassing => new("NVT", "Niet van toepassing");

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
