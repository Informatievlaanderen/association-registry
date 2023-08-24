namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Framework;

public record AfdelingWerdGeregistreerd(
    string VCode,
    string Naam,
    AfdelingWerdGeregistreerd.MoederverenigingsData Moedervereniging,
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    Registratiedata.Doelgroep Doelgroep,
    Registratiedata.Contactgegeven[] Contactgegevens,
    Registratiedata.Locatie[] Locaties,
    Registratiedata.Vertegenwoordiger[] Vertegenwoordigers,
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent
{
    [IgnoreDataMember]
    public string Bron
        => Vereniging.Bronnen.Bron.Initiator;
    public record MoederverenigingsData(string KboNummer, string VCode, string Naam);
}
