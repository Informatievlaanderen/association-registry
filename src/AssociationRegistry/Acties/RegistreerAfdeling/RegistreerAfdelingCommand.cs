namespace AssociationRegistry.Acties.RegistreerAfdeling;

using Vereniging;

public record RegistreerAfdelingCommand(
    VerenigingsNaam Naam,
    KboNummer KboNummerMoedervereniging,
    string? KorteNaam,
    string? KorteBeschrijving,
    Startdatum Startdatum,
    Contactgegeven[] Contactgegevens,
    Locatie[] Locaties,
    Vertegenwoordiger[] Vertegenwoordigers,
    HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket,
    bool SkipDuplicateDetection = false);
