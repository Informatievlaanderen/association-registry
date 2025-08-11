namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.WijzigLidmaatschap;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record WijzigLidmaatschapCommand(
    VCode VCode,
    TeWijzigenLidmaatschap Lidmaatschap);

