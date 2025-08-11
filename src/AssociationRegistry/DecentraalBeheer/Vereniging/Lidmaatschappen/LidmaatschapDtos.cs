namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public record ToeTeVoegenLidmaatschap(
    VCode AndereVereniging,
    string AndereVerenigingNaam,
    Geldigheidsperiode Geldigheidsperiode,
    LidmaatschapIdentificatie Identificatie,
    LidmaatschapBeschrijving Beschrijving);

public record TeWijzigenLidmaatschap(
    LidmaatschapId LidmaatschapId,
    GeldigVan? GeldigVan,
    GeldigTot? GeldigTot,
    LidmaatschapIdentificatie? Identificatie,
    LidmaatschapBeschrijving? Beschrijving);