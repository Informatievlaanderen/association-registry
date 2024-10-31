namespace AssociationRegistry.Acties.WijzigLidmaatschap;

using Vereniging;

public record WijzigLidmaatschapCommand(
    VCode VCode,
    WijzigLidmaatschapCommand.TeWijzigenLidmaatschap Lidmaatschap)
{
    public record TeWijzigenLidmaatschap(
        LidmaatschapId LidmaatschapId,
        Geldigheidsperiode Geldigheidsperiode,
        string Identificatie,
        string Beschrijving);
}

