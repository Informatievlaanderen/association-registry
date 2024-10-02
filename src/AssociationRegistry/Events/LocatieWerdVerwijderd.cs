namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record LocatieWerdVerwijderd(string VCode,
    Registratiedata.Locatie Locatie) : IEvent
{
    public static LocatieWerdVerwijderd With(VCode vCode, Locatie locatie)
        => new(vCode, Registratiedata.Locatie.With(locatie));
}
