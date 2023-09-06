namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record EinddatumWerdGewijzigd(DateOnly Einddatum) : IEvent
{
    public static EinddatumWerdGewijzigd With(Datum datum)
        => new(datum.Value);
}
