﻿namespace AssociationRegistry.Vereniging.RegistreerVereniging;

using DuplicateDetection;

public record PotentialDuplicatesFound(IEnumerable<DuplicaatVereniging> Candidates);
