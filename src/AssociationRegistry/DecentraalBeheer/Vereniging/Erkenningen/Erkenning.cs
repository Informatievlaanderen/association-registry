namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record Erkenning
{
    public int ErkenningId { get; set; }
    public string VCode { get; set; } = null!;
    public GegevensInitiator GeregistreerdDoor { get; set; }
    public IpdcProduct IpdcProduct { get; set; }

    public ErkenningsPeriode ErkenningsPeriode { get; set; }
    public DateOnly Hernieuwingsdatum { get; set; }
    public string HernieuwingsUrl { get; set; } = null!;
    public string Motivering { get; set; } = null!;
    public string Status { get; set; } = null!;

    public static Erkenning Create(
        int nextId,
        TeRegistrerenErkenning erkenning,
        IpdcProduct ipdcProduct,
        string initiator
    )
    {
        var erkenningsPeriode = ErkenningsPeriode.Create(erkenning.Startdatum, erkenning.Einddatum);

        var today = DateOnly.FromDateTime(DateTime.Today);

        return new Erkenning
        {
            ErkenningId = nextId,
            ErkenningsPeriode = erkenningsPeriode,
            Hernieuwingsdatum = erkenning.Hernieuwingsdatum,
            HernieuwingsUrl = erkenning.HernieuwingsUrl,
            IpdcProduct = ipdcProduct,
            GeregistreerdDoor = new GegevensInitiator { OvoCode = initiator },
            Status = ErkenningStatus.Bepaal(erkenningsPeriode, today),
        };
    }

    public static Erkenning Hydrate(
        int id,
        GegevensInitiator geregistreerdDoor,
        IpdcProduct ipdcProduct,
        DateOnly? startdatum,
        DateOnly? einddatum,
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
            ErkenningsPeriode = ErkenningsPeriode.Hydrate(startdatum, einddatum),
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
