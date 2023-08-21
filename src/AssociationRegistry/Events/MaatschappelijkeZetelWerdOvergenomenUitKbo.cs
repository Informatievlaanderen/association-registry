namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Framework;
using Vereniging;

public record MaatschappelijkeZetelWerdOvergenomenUitKbo(
    Registratiedata.Locatie Locatie) : IEvent
{
    [IgnoreDataMember]
    public string Bron
        => AssociationRegistry.Vereniging.Bronnen.Bron.KBO;
    public static MaatschappelijkeZetelWerdOvergenomenUitKbo With(Locatie locatie)
        => new(Registratiedata.Locatie.With(locatie));
}
