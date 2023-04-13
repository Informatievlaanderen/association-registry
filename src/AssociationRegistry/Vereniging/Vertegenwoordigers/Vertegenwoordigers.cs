namespace AssociationRegistry.Vereniging;

using Framework;
using Exceptions;

public class Vertegenwoordigers : List<Vertegenwoordiger>
{
    private Vertegenwoordigers(IEnumerable<Vertegenwoordiger> vertegenwoordigers)
    {
        AddRange(vertegenwoordigers);
    }
    public static Vertegenwoordigers Empty
        => new(Enumerable.Empty<Vertegenwoordiger>().ToList());

    public static Vertegenwoordigers FromArray(
        Vertegenwoordiger[] vertegenwoordigers)
    {
        Throw<DuplicateInszProvided>.If(HasDuplicateInsz(vertegenwoordigers));
        Throw<MultiplePrimaryContacts>.If(HasMultiplePrimaryContacts(vertegenwoordigers));

        return new Vertegenwoordigers(vertegenwoordigers);
    }

    private static bool HasMultiplePrimaryContacts(IEnumerable<Vertegenwoordiger> vertegenwoordigersArray)
        => vertegenwoordigersArray.Count(vertegenwoordiger => vertegenwoordiger.PrimairContactpersoon) > 1;

    private static bool HasDuplicateInsz(IReadOnlyCollection<Vertegenwoordiger> vertegenwoordigers)
        => vertegenwoordigers.DistinctBy(x => x.Insz).Count() != vertegenwoordigers.Count;


}
