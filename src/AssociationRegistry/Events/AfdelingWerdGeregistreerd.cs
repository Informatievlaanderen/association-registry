namespace AssociationRegistry.Events;

using Framework;

public record AfdelingWerdGeregistreerd(
    string VCode,
    string Naam,
    AfdelingWerdGeregistreerd.MoederverenigingsData Moedervereniging,
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    bool IsUitgeschrevenUitPubliekeDatastroom,
    Registratiedata.Contactgegeven[] Contactgegevens,
    Registratiedata.Locatie[] Locaties,
    Registratiedata.Vertegenwoordiger[] Vertegenwoordigers,
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent
{
    public record MoederverenigingsData(string KboNummer, string VCode, string Naam);
}
