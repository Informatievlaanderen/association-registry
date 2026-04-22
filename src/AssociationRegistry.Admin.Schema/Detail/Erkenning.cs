namespace AssociationRegistry.Admin.Schema.Detail;

public record Erkenning
{
    public JsonLdMetadata JsonLdMetadata { get; set; }
    public int ErkenningId { get; set; }
    public GegevensInitiator GeregistreerdDoor { get; set; } = null!;
    public IpdcProduct IpdcProduct { get; set; } = null!;
    public string? Startdatum { get; set; }
    public string? Einddatum { get; set; }
    public string? Hernieuwingsdatum { get; set; }
    public string HernieuwingsUrl { get; set; } = null!;
    public string RedenSchorsing { get; set; } = null!;
    public string Status { get; set; } = null!;
}
