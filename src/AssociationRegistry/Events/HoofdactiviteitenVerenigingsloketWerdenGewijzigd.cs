namespace AssociationRegistry.Events;

using Framework;

public record HoofdactiviteitenVerenigingsloketWerdenGewijzigd(HoofdactiviteitenVerenigingsloketWerdenGewijzigd.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent
{
    public record HoofdactiviteitVerenigingsloket(
        string Code,
        string Beschrijving)
    {
        public static HoofdactiviteitVerenigingsloket With(Vereniging.HoofdactiviteitVerenigingsloket activiteit)
            => new(activiteit.Code, activiteit.Beschrijving);
    }

    public static HoofdactiviteitenVerenigingsloketWerdenGewijzigd With(Vereniging.HoofdactiviteitVerenigingsloket[] hoofdactiviteitVerenigingslokets)
        => new(hoofdactiviteitVerenigingslokets.Select(HoofdactiviteitVerenigingsloket.With).ToArray());
}
