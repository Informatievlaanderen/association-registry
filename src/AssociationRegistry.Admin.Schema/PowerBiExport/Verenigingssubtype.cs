namespace AssociationRegistry.Admin.Schema.PowerBiExport;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record Verenigingssubtype : IVerenigingssubtypeCode
{
    public string Code { get; init; } = null!;
    public string Naam { get; init; } = null!;
}
