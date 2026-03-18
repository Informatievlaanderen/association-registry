namespace AssociationRegistry.Admin.Schema.PowerBiExport;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record Verenigingstype : IVerenigingstype
{
    public string Code { get; init; } = null!;
    public string Naam { get; init; } = null!;
}
