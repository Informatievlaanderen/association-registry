namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record MaatschappelijkeZetelWerdOvergenomenUitKbo(
    Registratiedata.Locatie Locatie) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;


}
