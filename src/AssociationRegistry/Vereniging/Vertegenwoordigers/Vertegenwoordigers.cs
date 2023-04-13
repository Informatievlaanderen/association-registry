namespace AssociationRegistry.Vereniging;

using System.Collections.ObjectModel;
using Framework;
using Exceptions;

public class Vertegenwoordigers : ReadOnlyCollection<Vertegenwoordiger>
{
    private Vertegenwoordigers(Vertegenwoordiger[] vertegenwoordigers) : base(vertegenwoordigers)
    {
    }

    public static Vertegenwoordigers Empty
        => new(Array.Empty<Vertegenwoordiger>());

    public static Vertegenwoordigers FromArray(Vertegenwoordiger[] vertegenwoordigers)
    {
        Throw<DuplicateInszProvided>.If(HasDuplicateInsz(vertegenwoordigers));
        Throw<MultiplePrimaryContacts>.If(HasMultiplePrimaryContacts(vertegenwoordigers));

        return new Vertegenwoordigers(vertegenwoordigers);
    }

    private static bool HasMultiplePrimaryContacts(Vertegenwoordiger[] vertegenwoordigersArray)
        => vertegenwoordigersArray.Count(vertegenwoordiger => vertegenwoordiger.PrimairContactpersoon) > 1;

    private static bool HasDuplicateInsz(Vertegenwoordiger[] vertegenwoordigers)
        => vertegenwoordigers.DistinctBy(x => x.Insz).Count() != vertegenwoordigers.Length;
}
