namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record FeitelijkeVerenigingWerdGeregistreerd(
    string VCode,
    string Naam,
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    Registratiedata.Doelgroep Doelgroep,
    bool IsUitgeschrevenUitPubliekeDatastroom,
    Registratiedata.Contactgegeven[] Contactgegevens,
    Registratiedata.Locatie[] Locaties,
    Registratiedata.Vertegenwoordiger[] Vertegenwoordigers,
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket,
    Registratiedata.Werkingsgebied[]? Werkingsgebieden = null) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}
