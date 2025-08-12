﻿namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VerwijderLidmaatschap;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record VerwijderLidmaatschapCommand(VCode VCode, LidmaatschapId LidmaatschapId);
