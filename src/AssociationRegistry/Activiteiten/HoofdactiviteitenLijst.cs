namespace AssociationRegistry.Activiteiten;

using System.Collections;
using System.Collections.Immutable;
using Exceptions;
using Framework;

public class HoofdactiviteitenLijst : IEnumerable<Hoofdactiviteit>
{
    private ImmutableList<Hoofdactiviteit> _hoofdactiviteiten;

    private HoofdactiviteitenLijst(IEnumerable<Hoofdactiviteit> listOfHoofdactiviteiten)
    {
        _hoofdactiviteiten = listOfHoofdactiviteiten.ToImmutableList();
    }

    public static HoofdactiviteitenLijst Empty
        => new(Array.Empty<Hoofdactiviteit>());

    public static HoofdactiviteitenLijst Create(IEnumerable<Hoofdactiviteit> listOfHoofdactiviteiten)
    {
        var list = listOfHoofdactiviteiten.ToList();
        Throw<DuplicateHoofdactiviteit>.If(HasDuplicateHoofdactiviteit(list));
        return new HoofdactiviteitenLijst(list);
    }

    private static bool HasDuplicateHoofdactiviteit(IReadOnlyCollection<Hoofdactiviteit> list)
    {
        return list.DistinctBy(x => x.Code).Count() != list.Count();
    }


    public IEnumerator<Hoofdactiviteit> GetEnumerator()
        => _hoofdactiviteiten.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
