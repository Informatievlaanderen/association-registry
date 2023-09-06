namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record StartdatumWerdGewijzigd(string VCode, DateOnly? Startdatum) : IEvent
{
    public static StartdatumWerdGewijzigd With(VCode vCode, Datum? startDatum)
        => new(vCode, startDatum?.Value ?? null);
}
