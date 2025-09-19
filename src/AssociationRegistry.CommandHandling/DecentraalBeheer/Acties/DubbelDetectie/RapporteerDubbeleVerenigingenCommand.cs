namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.DubbelDetectie;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.DuplicaatDetectie;

public record RapporteerDubbeleVerenigingenCommand(
    string Key,
    VerenigingsNaam Naam,
    Locatie[] Locaties,
    DuplicaatVereniging[] GedetecteerdeDubbels);
