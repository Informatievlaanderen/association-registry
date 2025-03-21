﻿namespace AssociationRegistry.DecentraalBeheer.Lidmaatschappen.WijzigLidmaatschap;

using AssociationRegistry.Vereniging;

public record WijzigLidmaatschapCommand(
    VCode VCode,
    WijzigLidmaatschapCommand.TeWijzigenLidmaatschap Lidmaatschap)
{
    public record TeWijzigenLidmaatschap(
        LidmaatschapId LidmaatschapId,
        GeldigVan? GeldigVan,
        GeldigTot? GeldigTot,
        LidmaatschapIdentificatie? Identificatie,
        LidmaatschapBeschrijving? Beschrijving);
}

