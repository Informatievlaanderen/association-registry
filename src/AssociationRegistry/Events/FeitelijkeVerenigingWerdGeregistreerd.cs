namespace AssociationRegistry.Events;

using Framework;

public record FeitelijkeVerenigingWerdGeregistreerd(
    string VCode,
    string Naam,
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    Registratiedata.Contactgegeven[] Contactgegevens,
    Registratiedata.Locatie[] Locaties,
    Registratiedata.Vertegenwoordiger[] Vertegenwoordigers,
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket) : IEvent;
