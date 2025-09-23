namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.DubbelDetectie;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;
using Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;

public record RapporteerDubbeleVerenigingenCommand(
    string Key,
    VerenigingsNaam Naam,
    Locatie[] Locaties,
    DuplicaatVereniging[] GedetecteerdeDubbels);
