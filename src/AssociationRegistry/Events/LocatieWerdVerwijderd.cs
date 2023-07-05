namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record LocatieWerdVerwijderd(
    Registratiedata.Locatie Locatie) : IEvent
{
    public static LocatieWerdVerwijderd With(Locatie locatie)
        => new(Registratiedata.Locatie.With(locatie));
}
