namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand(
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
    Werkingsgebied[] Werkingsgebieden,
    bool SkipDuplicateDetection = false,
    string Bevestigingstoken = "")
{
}
