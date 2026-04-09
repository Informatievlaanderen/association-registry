namespace AssociationRegistry.Admin.Schema.Detail;

public record Erkenning
{
    public JsonLdMetadata JsonLdMetadata { get; set; }

    public int ErkenningId { get; set; }
    public string VCode { get; set; } = null!;
    public GegevensInitiator GeregistreerdDoor { get; set; }
    public IpdcProduct? IpdcProduct { get; set; }
    public DateOnly Startdatum { get; set; }
    public DateOnly Einddatum { get; set; }
    public DateOnly Hernieuwingsdatum { get; set; }
    public string HernieuwingsUrl { get; set; } = null!;
    public string Motivering { get; set; } = null!;
}
