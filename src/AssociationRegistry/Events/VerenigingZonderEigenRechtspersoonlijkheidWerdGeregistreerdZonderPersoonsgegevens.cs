namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens(
    string VCode,
    string Naam,
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    Registratiedata.Doelgroep Doelgroep,
    bool IsUitgeschrevenUitPubliekeDatastroom,
    Registratiedata.Contactgegeven[] Contactgegevens,
    Registratiedata.Locatie[] Locaties,
    Registratiedata.VertegenwoordigerZonderPersoonsgegevens[] Vertegenwoordigers,
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket,
    Registratiedata.DuplicatieInfo? DuplicatieInfo) : IEvent, IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}
