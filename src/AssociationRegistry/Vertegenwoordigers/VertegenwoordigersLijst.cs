namespace AssociationRegistry.Vertegenwoordigers;

using Exceptions;
using Framework;

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
    {
        if (vertegenwoordigers is null)
            return Empty;

        var vertegenwoordigersArray = vertegenwoordigers as Vertegenwoordiger[] ?? vertegenwoordigers.ToArray();

        Throw<DuplicateInszProvided>.If(HasDuplicateInsz(vertegenwoordigersArray));

        return new VertegenwoordigersLijst(vertegenwoordigersArray);
    }

    private static bool HasDuplicateInsz(IReadOnlyCollection<Vertegenwoordiger> vertegenwoordigers)
        => vertegenwoordigers.DistinctBy(x => x.Insz).Count() != vertegenwoordigers.Count;

    public static VertegenwoordigersLijst Empty
        => new();
}
