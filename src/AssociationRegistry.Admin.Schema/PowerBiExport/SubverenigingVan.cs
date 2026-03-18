namespace AssociationRegistry.Admin.Schema.PowerBiExport;

public record SubverenigingVan
{
    public string AndereVereniging { get; init; } = null!;
    public string AndereVerenigingNaam { get; init; } = null!;
    public string Identificatie { get; init; } = null!;
    public string Beschrijving { get; init; } = null!;
}
