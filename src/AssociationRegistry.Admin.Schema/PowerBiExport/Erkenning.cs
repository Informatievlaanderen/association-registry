namespace AssociationRegistry.Admin.Schema.PowerBiExport;

public record Erkenning
{
    public int ErkenningId { get; set; }
    public GegevensInitiator GeregistreerdDoor { get; set; } = null!;
    public IpdcProduct IpdcProduct { get; set; } = null!;
    public string Startdatum { get; set; } = null!;
    public string Einddatum { get; set; } = null!;
    public string Hernieuwingsdatum { get; set; } = null!;
    public string HernieuwingsUrl { get; set; } = null!;
    public string RedenSchorsing { get; set; } = null!;
    public string Status { get; set; } = null!;
}
