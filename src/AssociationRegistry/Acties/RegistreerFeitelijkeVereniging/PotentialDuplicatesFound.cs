﻿namespace AssociationRegistry.Acties.RegistreerFeitelijkeVereniging;

using DuplicateVerenigingDetection;

public record PotentialDuplicatesFound(IEnumerable<DuplicaatVereniging> Candidates);
