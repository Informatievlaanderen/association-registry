namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;
using Vereniging;
using Vereniging.Bronnen;

public record MaatschappelijkeZetelWerdGewijzigdInKbo(
    Registratiedata.Locatie Locatie) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;

    public static MaatschappelijkeZetelWerdGewijzigdInKbo With(Locatie locatie)
        => new(Registratiedata.Locatie.With(locatie));
}
