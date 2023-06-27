namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record LocatieWerdToegevoegd(
    Registratiedata.Locatie Locatie) : IEvent
{
    public static LocatieWerdToegevoegd With(Locatie locatie)
        => new(Registratiedata.Locatie.With(locatie));
}
