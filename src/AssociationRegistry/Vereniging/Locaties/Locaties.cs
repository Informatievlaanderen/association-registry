namespace AssociationRegistry.Vereniging;

public class Locaties : List<Locatie>
{
    private Locaties(IEnumerable<Locatie> locaties)
    {
        AddRange(locaties);
    }

    public static Locaties Empty
        => new(Enumerable.Empty<Locatie>().ToList());

    public static Locaties FromArray(Locatie[] locaties)
        => new(locaties);
}
