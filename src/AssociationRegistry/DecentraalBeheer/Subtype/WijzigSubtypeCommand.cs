namespace AssociationRegistry.DecentraalBeheer.Subtype;

using Vereniging;

public record WijzigSubtypeCommand(
    VCode VCode,
    WijzigSubtypeCommand.TeWijzigenSubtype SubtypeData): IWijzigSubtypeCommand
{
    public record TeWijzigenSubtype(
        VCode? AndereVereniging,
        string? AndereVerenigingNaam,
        SubtypeIdentificatie? Identificatie,
        SubtypeBeschrijving? Beschrijving);
}
