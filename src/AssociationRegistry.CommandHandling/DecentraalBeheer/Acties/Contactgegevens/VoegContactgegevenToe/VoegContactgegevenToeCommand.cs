﻿namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.VoegContactgegevenToe;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record VoegContactgegevenToeCommand(VCode VCode, Contactgegeven Contactgegeven);
