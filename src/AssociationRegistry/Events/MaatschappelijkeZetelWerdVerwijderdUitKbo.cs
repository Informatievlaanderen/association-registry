namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record MaatschappelijkeZetelWerdVerwijderdUitKbo(
    Registratiedata.Locatie Locatie) : IEvent
{
    public static MaatschappelijkeZetelWerdVerwijderdUitKbo With(Locatie locatie)
        => new(Registratiedata.Locatie.With(locatie));
}
