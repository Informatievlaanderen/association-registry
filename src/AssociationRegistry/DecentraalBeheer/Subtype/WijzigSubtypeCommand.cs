namespace AssociationRegistry.DecentraalBeheer.Subtype;

using Vereniging;

public record WijzigSubtypeCommand(
    VCode VCode,
    IWijzigSubtypeCommand SubtypeData)
{
    public record TeWijzigenSubtype(
        Subtype Subtype,
        VCode? AndereVereniging,
        string? AndereVerenigingNaam,
        SubtypeIdentificatie? Identificatie,
        SubtypeBeschrijving? Beschrijving) : IWijzigSubtypeCommand;

    public record TeWijzigenNaarFeitelijkeVereniging(Subtype Subtype) : IWijzigSubtypeCommand;
    public record TerugTeZettenNaarNogNietBepaald(Subtype Subtype) : IWijzigSubtypeCommand;
}

public interface IWijzigSubtypeCommand
{
    public Subtype Subtype { get; init; }
}
