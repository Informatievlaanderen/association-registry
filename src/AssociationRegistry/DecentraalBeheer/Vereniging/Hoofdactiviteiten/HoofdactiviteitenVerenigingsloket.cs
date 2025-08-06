namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Framework;
using Exceptions;
using System.Collections.ObjectModel;

public class HoofdactiviteitenVerenigingsloket : ReadOnlyCollection<HoofdactiviteitVerenigingsloket>
{
    private HoofdactiviteitenVerenigingsloket(HoofdactiviteitVerenigingsloket[] hoofdactiviteiten) : base(hoofdactiviteiten)
    {
    }

    public static HoofdactiviteitenVerenigingsloket Empty
        => new(Array.Empty<HoofdactiviteitVerenigingsloket>());

    public static HoofdactiviteitenVerenigingsloket FromArray(HoofdactiviteitVerenigingsloket[] hoofdactiviteiten)
    {
        Throw<HoofdactiviteitIsDuplicaat>.If(HasDuplicateHoofdactiviteit(hoofdactiviteiten));

        return new HoofdactiviteitenVerenigingsloket(hoofdactiviteiten);
    }

    private static bool HasDuplicateHoofdactiviteit(HoofdactiviteitVerenigingsloket[] hoofdactiviteiten)
    {
        return hoofdactiviteiten.DistinctBy(x => x.Code).Count() != hoofdactiviteiten.Length;
    }

    public static HoofdactiviteitenVerenigingsloket Hydrate(HoofdactiviteitVerenigingsloket[] hoofdactiviteiten)
        => new(hoofdactiviteiten);

    public static implicit operator HoofdactiviteitVerenigingsloket[](HoofdactiviteitenVerenigingsloket hoofdactiviteitenVerenigingsloket)
        => hoofdactiviteitenVerenigingsloket.ToArray();

    public static bool Equals(
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket1,
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket2)
        => hoofdactiviteitenVerenigingsloket1.Length == hoofdactiviteitenVerenigingsloket2.Length &&
           hoofdactiviteitenVerenigingsloket1.All(hoofdactiviteitenVerenigingsloket2.Contains);
}
