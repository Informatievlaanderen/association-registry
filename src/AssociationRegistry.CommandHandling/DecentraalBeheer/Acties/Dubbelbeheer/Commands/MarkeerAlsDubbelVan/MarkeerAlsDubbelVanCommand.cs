﻿namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record MarkeerAlsDubbelVanCommand(VCode VCode, VCode VCodeAuthentiekeVereniging);
