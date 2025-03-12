namespace AssociationRegistry.Admin.Schema.Detail;

public record SubverenigingVan
{
    public string AndereVereniging { get; init; } = null!;
    public string AndereVerenigingNaam { get; init; } = null!;
    public string Identificatie { get; init; } = null!;
    public string Beschrijving { get; init; } = null!;
}
