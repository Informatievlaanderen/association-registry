namespace AssociationRegistry.Acties.VerwijderLidmaatschap;

using Vereniging;

public record VerwijderLidmaatschapCommand(VCode VCode, int LidmaatschapId);
