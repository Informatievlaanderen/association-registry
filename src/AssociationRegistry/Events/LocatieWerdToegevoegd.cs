namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;
using Vereniging;
using Vereniging.Bronnen;

public record LocatieWerdToegevoegd(
    Registratiedata.Locatie Locatie) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;

    public static LocatieWerdToegevoegd With(Locatie locatie)
        => new(Registratiedata.Locatie.With(locatie));
}
