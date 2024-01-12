namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent
{
    public static HoofdactiviteitenVerenigingsloketWerdenGewijzigd With(
        IEnumerable<HoofdactiviteitVerenigingsloket> hoofdactiviteitenVerenigingsloket)
        => new(hoofdactiviteitenVerenigingsloket
              .Select(Registratiedata.HoofdactiviteitVerenigingsloket.With)
              .ToArray());
}
