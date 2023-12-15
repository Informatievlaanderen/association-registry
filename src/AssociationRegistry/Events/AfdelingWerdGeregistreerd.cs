namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;
using Vereniging.Bronnen;

[Obsolete]
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
    public Bron Bron
        => Bron.Initiator;
    public record MoederverenigingsData(string KboNummer, string VCode, string Naam);
}
