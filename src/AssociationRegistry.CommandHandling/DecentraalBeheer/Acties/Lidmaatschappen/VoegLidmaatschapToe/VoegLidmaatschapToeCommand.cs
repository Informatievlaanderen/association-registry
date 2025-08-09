namespace AssociationRegistry.DecentraalBeheer.Acties.Lidmaatschappen.VoegLidmaatschapToe;

using Vereniging;

public record VoegLidmaatschapToeCommand(
    VCode VCode,
    VoegLidmaatschapToeCommand.ToeTeVoegenLidmaatschap Lidmaatschap)
{
    public record ToeTeVoegenLidmaatschap(
        VCode AndereVereniging,
        string AndereVerenigingNaam,
        Geldigheidsperiode Geldigheidsperiode,
        LidmaatschapIdentificatie Identificatie,
        LidmaatschapBeschrijving Beschrijving);
}

