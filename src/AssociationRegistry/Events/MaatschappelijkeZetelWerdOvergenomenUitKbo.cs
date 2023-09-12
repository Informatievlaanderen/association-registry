namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Framework;
using Vereniging;
using Vereniging.Bronnen;

public record MaatschappelijkeZetelWerdOvergenomenUitKbo(
    Registratiedata.Locatie Locatie) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;
    public static MaatschappelijkeZetelWerdOvergenomenUitKbo With(Locatie locatie)
        => new(Registratiedata.Locatie.With(locatie));
}
