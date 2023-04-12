namespace AssociationRegistry.Vereniging.RegistreerVereniging;

using Contactgegevens;
using Hoofdactiviteiten;
using KboNummers;
using Locaties;
using Primitives;
using Startdatums;
using VerenigingsNamen;
using Vertegenwoordigers;

public record RegistreerVerenigingCommand(
    VerenigingsNaam Naam,
    string? KorteNaam,
    string? KorteBeschrijving,
    Startdatum Startdatum,
    KboNummer? KboNummer,
    Contactgegeven[] Contactgegevens,
    Locatie[] Locaties,
    Vertegenwoordiger[] Vertegenwoordigers,
    HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket,
    bool SkipDuplicateDetection = false);
