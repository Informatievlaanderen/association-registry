namespace AssociationRegistry.DecentraalBeheer.Acties.Lidmaatschappen.VerwijderLidmaatschap;

using Vereniging;

public record VerwijderLidmaatschapCommand(VCode VCode, LidmaatschapId LidmaatschapId);
