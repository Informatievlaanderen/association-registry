namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record StartdatumWerdGewijzigd(string VCode, DateOnly? Startdatum) : IEvent
{
    public static StartdatumWerdGewijzigd With(VCode vCode, Datum? startDatum)
        => new(vCode, startDatum?.Value ?? null);
}

public record StartdatumWerdGewijzigdInKbo(DateOnly? Startdatum) : IEvent
{
    public static StartdatumWerdGewijzigdInKbo With(Datum? startDatum)
        => new(startDatum?.Value ?? null);
}
