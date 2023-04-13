namespace AssociationRegistry.Acties.RegistreerVereniging;

using DuplicateVerenigingDetection;

public record PotentialDuplicatesFound(IEnumerable<DuplicaatVereniging> Candidates);
