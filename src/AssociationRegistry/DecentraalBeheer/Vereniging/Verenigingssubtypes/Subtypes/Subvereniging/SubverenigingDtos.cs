namespace AssociationRegistry.DecentraalBeheer.Vereniging.Subtypes.Subvereniging;

public record SubverenigingVanDto(
    VCode? AndereVereniging,
    SubverenigingIdentificatie? Identificatie,
    SubverenigingBeschrijving? Beschrijving)
{
    public string AndereVerenigingNaam { get; set; } = string.Empty;
};