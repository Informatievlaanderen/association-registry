namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record Erkenning
{
    public int ErkenningId { get; set; }
    public string VCode { get; set; } = null!;
    public GegevensInitiator GeregistreerdDoor { get; set; }
    public IpdcProduct IpdcProduct { get; set; }
    public DateOnly Startdatum { get; set; }
    public DateOnly Einddatum { get; set; }
    public DateOnly Hernieuwingsdatum { get; set; }
    public string HernieuwingsUrl { get; set; } = null!;
    public string Motivering { get; set; } = null!;
    public string Status { get; set; } = null!;

    public static Erkenning Create(
        int nextId,
        TeRegistrerenErkenning erkenning,
        IpdcProduct ipdcProduct,
        string initiator
    ) =>
        new()
        {
            ErkenningId = nextId,
            Startdatum = erkenning.Startdatum,
            Einddatum = erkenning.Einddatum,
            Hernieuwingsdatum = erkenning.Hernieuwingsdatum,
            HernieuwingsUrl = erkenning.HernieuwingsUrl,
            IpdcProduct = ipdcProduct,
            GeregistreerdDoor = new GegevensInitiator() { OvoCode = initiator },
            Status = ErkenningStatus.Calculate(erkenning.Startdatum, erkenning.Einddatum)
        };

    public static Erkenning Hydrate(
        int id,
        GegevensInitiator geregistreerdDoor,
        IpdcProduct ipdcProduct,
        DateOnly startdatum,
        DateOnly einddatum,
        DateOnly hernieuwingsdatum,
        string hernieuwingsUrl,
        string motivering,
        string status
    ) =>
        new()
        {
            ErkenningId = id,
            GeregistreerdDoor = geregistreerdDoor,
            IpdcProduct = ipdcProduct,
            Startdatum = startdatum,
            Einddatum = einddatum,
            Hernieuwingsdatum = hernieuwingsdatum,
            HernieuwingsUrl = hernieuwingsUrl,
            Motivering = motivering,
            Status = status,
        };
}

public record IpdcProduct
{
    public string Nummer { get; set; } = null!;
    public string Naam { get; set; } = null!;
}

public record GegevensInitiator
{
    public string OvoCode { get; set; } = null!;
    public string Naam { get; set; }
}
