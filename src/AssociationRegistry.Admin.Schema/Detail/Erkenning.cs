namespace AssociationRegistry.Admin.Schema.Detail;

public record Erkenning
{
    public JsonLdMetadata JsonLdMetadata { get; set; }
    public int ErkenningId { get; set; }
    public GegevensInitiator GeregistreerdDoor { get; set; } = null!;
    public IpdcProduct IpdcProduct { get; set; } = null!;
    public string Startdatum { get; set; } = string.Empty;
    public string Einddatum { get; set; } = string.Empty;
    public string Hernieuwingsdatum { get; set; } = string.Empty;
    public string HernieuwingsUrl { get; set; } = string.Empty;
    public string RedenSchorsing { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
