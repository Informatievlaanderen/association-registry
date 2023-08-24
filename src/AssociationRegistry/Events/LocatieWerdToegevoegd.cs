namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;
using Vereniging;

public record LocatieWerdToegevoegd(
    Registratiedata.Locatie Locatie) : IEvent
{
    [IgnoreDataMember]
    public string Bron
        => AssociationRegistry.Vereniging.Bronnen.Bron.Initiator;

    public static LocatieWerdToegevoegd With(Locatie locatie)
        => new(Registratiedata.Locatie.With(locatie));
}
