namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging;
using Vereniging.Bronnen;

public record MaatschappelijkeZetelWerdGewijzigdInKbo(
    Registratiedata.Locatie Locatie) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;


}
