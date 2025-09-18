namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.DubbelDetectie;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.DuplicaatDetectie;
using Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;

public record RapporteerDubbeleVerenigingenCommand(
    VerenigingsNaam Naam,
    Locatie[] Locaties,
    DuplicaatVereniging[] GedetecteerdeDubbels);
