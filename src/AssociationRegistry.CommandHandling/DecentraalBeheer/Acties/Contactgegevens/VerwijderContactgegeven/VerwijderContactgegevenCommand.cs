﻿namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.VerwijderContactgegeven;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record VerwijderContactgegevenCommand(VCode VCode, int ContactgegevenId);
