namespace AssociationRegistry.Vereniging;

public class LocatieLijst : List<Locatie>
{
    private LocatieLijst(IEnumerable<Locatie> locaties)
    {
        AddRange(locaties);
    }

    private LocatieLijst()
    {
    }

    public static LocatieLijst Empty
        => new();

    public static LocatieLijst CreateInstance(IEnumerable<Locatie>? locaties)
        => locaties is null ? Empty : new LocatieLijst(locaties);
}
