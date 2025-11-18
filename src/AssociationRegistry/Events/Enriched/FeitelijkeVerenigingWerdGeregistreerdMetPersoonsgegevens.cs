namespace AssociationRegistry.Events.Enriched;

using Vereniging.Bronnen;
using System.Runtime.Serialization;

public record FeitelijkeVerenigingWerdGeregistreerdMetPersoonsgegevens(
    string VCode,
    string Naam,
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    Registratiedata.Doelgroep Doelgroep,
    bool IsUitgeschrevenUitPubliekeDatastroom,
    Registratiedata.Contactgegeven[] Contactgegevens,
    Registratiedata.Locatie[] Locaties,
    EnrichedVertegenwoordiger[] Vertegenwoordigers,
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent, IVerenigingWerdGeregistreerdMetPersoonsgegevens
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}
