namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public record RegistratieDataVerenigingZonderEigenRechtspersoonlijkheid(
    VerenigingsNaam Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    Datum? Startdatum,
    Doelgroep Doelgroep,
    bool IsUitgeschrevenUitPubliekeDatastroom,
    Contactgegeven[] Contactgegevens,
    Locatie[] Locaties,
    Vertegenwoordiger[] Vertegenwoordigers,
    HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket,
    Werkingsgebied[] Werkingsgebieden);