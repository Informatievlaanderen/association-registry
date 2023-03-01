namespace AssociationRegistry.Vereniging.RegistreerVereniging;

using DuplicateDetection;

public record PotentialDuplicatesFound(IEnumerable<DuplicateCandidate> Candidates);
