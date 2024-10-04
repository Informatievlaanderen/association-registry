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

    public static Werkingsgebied Parse(string code)
    {
        return All.Single(x => x.Code == code);
    }

    public static Werkingsgebied[] All =
    [
        new("BE", "België"),
        new("BE2", "Vlaams gewest"),
        new("BE25", "West-Vlaanderen"),
        new("BE255", "Arrondissement Oostende"),
        new("BE25535002", "Bredene"),

        new("BE3", "Brussels Hoofdstedelijk Gewest"),
        new("BE33", "Brussels"),
        new("BE331", "Brussels Hoofdstad"),
        new("BE33101001", "Elsene"),

        new("BE1", "Waals gewest"),
        new("BE32", "Namen"),
        new("BE325", "Arrondissement Namen"),
        new("BE32594045", "Dinant"),
    ];
}
