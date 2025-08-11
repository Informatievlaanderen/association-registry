namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VoegLidmaatschapToe;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record VoegLidmaatschapToeCommand(
    VCode VCode,
    ToeTeVoegenLidmaatschap Lidmaatschap);

