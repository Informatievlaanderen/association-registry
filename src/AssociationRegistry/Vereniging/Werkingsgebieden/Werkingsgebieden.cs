namespace AssociationRegistry.Vereniging;

using Exceptions;
using Framework;
using System.Collections.ObjectModel;

public class Werkingsgebieden : ReadOnlyCollection<Werkingsgebied>
{
    private Werkingsgebieden(Werkingsgebied[] werkingsgebieden) : base(werkingsgebieden)
    {
    }

    public static Werkingsgebieden Empty
        => new(Array.Empty<Werkingsgebied>());

    public static Werkingsgebieden FromArray(Werkingsgebied[] werkingsgebieden)
    {
        Throw<WerkingsgebiedIsDuplicaat>.If(HasDuplicateWerkingsgebied(werkingsgebieden));

        return new Werkingsgebieden(werkingsgebieden);
    }

    private static bool HasDuplicateWerkingsgebied(Werkingsgebied[] werkingsgebieden)
    {
        return werkingsgebieden.DistinctBy(x => x.Code).Count() != werkingsgebieden.Length;
    }

    public static Werkingsgebieden Hydrate(Werkingsgebied[] werkingsgebieden)
        => new(werkingsgebieden);

    public static implicit operator Werkingsgebied[](Werkingsgebieden werkingsgebieden)
        => werkingsgebieden.ToArray();

    public static bool Equals(
        Werkingsgebied[] werkingsgebieden1,
        Werkingsgebied[] werkingsgebieden2)
        => werkingsgebieden1.Length == werkingsgebieden2.Length &&
           werkingsgebieden1.All(werkingsgebieden2.Contains);
}
