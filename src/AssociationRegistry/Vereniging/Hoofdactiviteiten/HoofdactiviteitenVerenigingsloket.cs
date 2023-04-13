namespace AssociationRegistry.Vereniging;

using System.Collections.ObjectModel;
using Framework;
using Exceptions;

public class HoofdactiviteitenVerenigingsloket : ReadOnlyCollection<HoofdactiviteitVerenigingsloket>
{
    private HoofdactiviteitenVerenigingsloket(HoofdactiviteitVerenigingsloket[] hoofdactiviteiten) : base(hoofdactiviteiten)
    {
    }

    public static HoofdactiviteitenVerenigingsloket Empty
        => new(Array.Empty<HoofdactiviteitVerenigingsloket>());

    public static HoofdactiviteitenVerenigingsloket FromArray(HoofdactiviteitVerenigingsloket[] hoofdactiviteiten)
    {
        Throw<DuplicateHoofdactiviteit>.If(HasDuplicateHoofdactiviteit(hoofdactiviteiten));

        return new HoofdactiviteitenVerenigingsloket(hoofdactiviteiten);
    }

    private static bool HasDuplicateHoofdactiviteit(HoofdactiviteitVerenigingsloket[] hoofdactiviteiten)
    {
        return hoofdactiviteiten.DistinctBy(x => x.Code).Count() != hoofdactiviteiten.Length;
    }
}
