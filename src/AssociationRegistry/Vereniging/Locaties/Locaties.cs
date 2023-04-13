namespace AssociationRegistry.Vereniging;

using System.Collections.ObjectModel;

public class Locaties : ReadOnlyCollection<Locatie>
{
    private Locaties(Locatie[] locaties):base(locaties)
    {
    }

    public static Locaties Empty
        => new(Array.Empty<Locatie>());

    public static Locaties FromArray(Locatie[] locaties)
        => new(locaties);
}
