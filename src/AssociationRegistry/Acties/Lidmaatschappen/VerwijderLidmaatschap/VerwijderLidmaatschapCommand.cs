namespace AssociationRegistry.Acties.Lidmaatschappen.VerwijderLidmaatschap;

using AssociationRegistry.Vereniging;

public record VerwijderLidmaatschapCommand(VCode VCode, LidmaatschapId LidmaatschapId);
