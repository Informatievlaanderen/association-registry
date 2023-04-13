namespace AssociationRegistry.Vereniging;

using Framework;
using Exceptions;

public class HoofdactiviteitenVerenigingsloket : List<HoofdactiviteitVerenigingsloket>
{
    private HoofdactiviteitenVerenigingsloket(IEnumerable<HoofdactiviteitVerenigingsloket> hoofdactiviteiten)
    {
        AddRange(hoofdactiviteiten);
    }

    public static HoofdactiviteitenVerenigingsloket Empty
        => new(Array.Empty<HoofdactiviteitVerenigingsloket>());

    public static HoofdactiviteitenVerenigingsloket FromArray(HoofdactiviteitVerenigingsloket[] hoofdactiviteiten)
    {
        var list = hoofdactiviteiten.ToList();
        Throw<DuplicateHoofdactiviteit>.If(HasDuplicateHoofdactiviteit(list));

        return new HoofdactiviteitenVerenigingsloket(list);
    }

    private static bool HasDuplicateHoofdactiviteit(IReadOnlyCollection<HoofdactiviteitVerenigingsloket> hoofdactiviteiten)
    {
        return hoofdactiviteiten.DistinctBy(x => x.Code).Count() != hoofdactiviteiten.Count;
    }
}
