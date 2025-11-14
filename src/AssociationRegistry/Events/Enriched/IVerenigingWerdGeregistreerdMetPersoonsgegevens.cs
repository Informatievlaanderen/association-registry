namespace AssociationRegistry.Events.Enriched;

public interface IVerenigingWerdGeregistreerdMetPersoonsgegevens
{
    string VCode { get; init; }
    string Naam { get; init; }
    string KorteNaam { get; init; }
    string KorteBeschrijving { get; init; }
    DateOnly? Startdatum { get; init; }
    Registratiedata.Doelgroep Doelgroep { get; init; }
    bool IsUitgeschrevenUitPubliekeDatastroom { get; init; }
    Registratiedata.Contactgegeven[] Contactgegevens { get; init; }
    Registratiedata.Locatie[] Locaties { get; init; }
    EnrichedVertegenwoordiger[] Vertegenwoordigers { get; init; }
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; }
}
