namespace AssociationRegistry.Hoofdactiviteiten;

using System.Collections;
using System.Collections.Immutable;
using Framework;
using Exceptions;

public class HoofdactiviteitenVerenigingsloketLijst : IEnumerable<HoofdactiviteitVerenigingsloket>
{
    private ImmutableList<HoofdactiviteitVerenigingsloket> _hoofdactiviteiten;

    private HoofdactiviteitenVerenigingsloketLijst(IEnumerable<HoofdactiviteitVerenigingsloket> listOfHoofdactiviteiten)
    {
        _hoofdactiviteiten = listOfHoofdactiviteiten.ToImmutableList();
    }

    public static HoofdactiviteitenVerenigingsloketLijst Empty
        => new(Array.Empty<HoofdactiviteitVerenigingsloket>());

    public static HoofdactiviteitenVerenigingsloketLijst Create(IEnumerable<HoofdactiviteitVerenigingsloket> listOfHoofdactiviteiten)
    {
        var list = listOfHoofdactiviteiten.ToList();
        Throw<DuplicateHoofdactiviteit>.If(HasDuplicateHoofdactiviteit(list));
        return new HoofdactiviteitenVerenigingsloketLijst(list);
    }

    private static bool HasDuplicateHoofdactiviteit(IReadOnlyCollection<HoofdactiviteitVerenigingsloket> list)
    {
        return list.DistinctBy(x => x.Code).Count() != list.Count();
    }


    public IEnumerator<HoofdactiviteitVerenigingsloket> GetEnumerator()
        => _hoofdactiviteiten.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
