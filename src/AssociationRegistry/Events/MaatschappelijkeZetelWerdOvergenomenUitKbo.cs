namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record MaatschappelijkeZetelWerdOvergenomenUitKbo(
    Registratiedata.Locatie Locatie) : IEvent
{
    public static MaatschappelijkeZetelWerdOvergenomenUitKbo With(Locatie locatie)
        => new(Registratiedata.Locatie.With(locatie));
}
