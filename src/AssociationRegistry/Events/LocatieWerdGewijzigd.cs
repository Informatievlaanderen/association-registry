namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record LocatieWerdGewijzigd(
    Registratiedata.Locatie Locatie) : IEvent
{
    public static LocatieWerdGewijzigd With(Locatie locatie)
        => new(Registratiedata.Locatie.With(locatie));
}
