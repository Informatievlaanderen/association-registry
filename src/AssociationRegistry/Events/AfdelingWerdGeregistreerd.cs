namespace AssociationRegistry.Events;

using Framework;

public record AfdelingWerdGeregistreerd(
    string VCode,
    string Naam,
    string KboNummerMoedervereniging,
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    Registratiedata.Contactgegeven[] Contactgegevens,
    Registratiedata.Locatie[] Locaties,
    Registratiedata.Vertegenwoordiger[] Vertegenwoordigers,
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent;
