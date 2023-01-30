namespace AssociationRegistry.Vertegenwoordigers;

public class VertegenwoordigersLijst  : List<Vertegenwoordiger>
{
    private VertegenwoordigersLijst(IEnumerable<Vertegenwoordiger> vertegenwoordigers)
    {
        AddRange(vertegenwoordigers);
    }

    private VertegenwoordigersLijst()
    {

    }

    public static VertegenwoordigersLijst Create(
        IEnumerable<Vertegenwoordiger>? vertegenwoordigers)
        => vertegenwoordigers is null ? Empty : new VertegenwoordigersLijst(vertegenwoordigers);

    public static VertegenwoordigersLijst Empty
        => new();
}
