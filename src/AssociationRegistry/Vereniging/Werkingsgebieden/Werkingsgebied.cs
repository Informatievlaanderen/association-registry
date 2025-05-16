namespace AssociationRegistry.Vereniging;

using Exceptions;

public record Werkingsgebied
{
    public static readonly Werkingsgebied[] ProvincieWerkingsgebieden = new[]
    {
        new Werkingsgebied("BE10", "Brussels Hoofdstedelijk Gewest"),
        new Werkingsgebied("BE21", "Provincie Antwerpen"),
        new Werkingsgebied("BE22", "Provincie Limburg"),
        new Werkingsgebied("BE23", "Provincie Oost-Vlaanderen"),
        new Werkingsgebied("BE24", "Provincie Vlaams-Brabant"),
        new Werkingsgebied("BE25", "Provincie West-Vlaanderen"),
    };

    static Werkingsgebied()
    {
        AllExamples = ProvincieWerkingsgebieden
             .Union(NutsLauReader
                   .Read()
                   .Select(x => new Werkingsgebied($"{x.Nuts}{x.Lau}", x.Gemeente)))
             .ToArray();

        AllWithNVTExamples = new[]
                     {
                         NietVanToepassing,
                     }
                    .Union(AllExamples)
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



    public static Werkingsgebied Hydrate(string code, string naam)
        => new Werkingsgebied(code, naam);

    public static Werkingsgebied[] AllWithNVTExamples { get; }
    public static Werkingsgebied[] AllExamples { get; }
}
