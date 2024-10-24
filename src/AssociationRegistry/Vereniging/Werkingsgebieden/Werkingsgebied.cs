namespace AssociationRegistry.Vereniging;

using Exceptions;

public class Werkingsgebied
{
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

    public static Werkingsgebied[] All =
    [
        new("BE", "België"),
        new("BE1", "Brussels Gewest"),
        new("BE2", "Vlaams Gewest"),
        new("BE21", "Provincie Antwerpen"),
        new("BE22", "Provincie Limburg"),
        new("BE25", "Provincie West-Vlaanderen"),
        new("BE212", "Arrondissement Machelen"),
        new("BE213", "Distriuct Turnhout"),
        new("BE255", "District Oostende"),
        new("BE25535002", "Bredene"),
    ];

    public static Dictionary<string, Werkingsgebied[]> Levels = new()
    {
        { "Nuts-0", All.Where(s => s.Code == "BE").ToArray() },
        { "Nuts-1", All.Where(w => w.Code.Length == 3).ToArray() },
        { "Nuts-2", All.Where(w => w.Code.Length == 4).ToArray() },
        { "Nuts-3", All.Where(w => w.Code.Length == 5).ToArray() },
        { "Lau", All.Where(w => w.Code.Length == 10).ToArray() },
    };
}
