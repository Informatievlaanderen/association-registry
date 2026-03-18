namespace AssociationRegistry.Admin.Schema.PowerBiExport;

public record Sleutel
{
    public string Bron { get; init; } = null!;
    public string Waarde { get; init; } = null!;
    public GestructureerdeIdentificator GestructureerdeIdentificator { get; set; }
    public string CodeerSysteem { get; set; }
}
