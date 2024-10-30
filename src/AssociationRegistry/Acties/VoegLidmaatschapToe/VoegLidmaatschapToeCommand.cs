namespace AssociationRegistry.Acties.VoegLidmaatschapToe;

using Vereniging;

public record VoegLidmaatschapToeCommand(VCode VCode, Lidmaatschap Lidmaatschap);
