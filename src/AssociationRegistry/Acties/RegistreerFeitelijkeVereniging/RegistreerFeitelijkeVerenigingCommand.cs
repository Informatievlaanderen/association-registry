namespace AssociationRegistry.Acties.RegistreerFeitelijkeVereniging;

using Vereniging;

public record RegistreerFeitelijkeVerenigingCommand(
    VerenigingsNaam Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    Startdatum Startdatum,
    Contactgegeven[] Contactgegevens,
    Locatie[] Locaties,
    Vertegenwoordiger[] Vertegenwoordigers,
    HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket,
    bool SkipDuplicateDetection = false);
