namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record Erkenning
{
    public int ErkenningId { get; set; }
    public GegevensInitiator GeregistreerdDoor { get; set; }
    public IpdcProduct IpdcProduct { get; set; }
    public ErkenningsPeriode ErkenningsPeriode { get; set; }
    public Hernieuwingsdatum Hernieuwingsdatum { get; set; }
    public HernieuwingsUrl HernieuwingsUrl { get; set; } = null!;
    public string RedenSchorsing { get; set; } = null!;
    public ErkenningStatus Status { get; set; } = null!;

    public static Erkenning Create(
        int nextId,
        TeRegistrerenErkenning erkenning,
        IpdcProduct ipdcProduct,
        GegevensInitiator initiator
    )
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        return new Erkenning
        {
            ErkenningId = nextId,
            ErkenningsPeriode = erkenning.ErkenningsPeriode,
            Hernieuwingsdatum = erkenning.Hernieuwingsdatum,
            HernieuwingsUrl = erkenning.HernieuwingsUrl,
            IpdcProduct = ipdcProduct,
            GeregistreerdDoor = initiator,
            Status = ErkenningStatus.Bepaal(erkenning.ErkenningsPeriode, today),
        };
    }

    public static Erkenning Hydrate(
        int id,
        GegevensInitiator geregistreerdDoor,
        IpdcProduct ipdcProduct,
        DateOnly? startdatum,
        DateOnly? einddatum,
        DateOnly? hernieuwingsdatum,
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
            Hernieuwingsdatum = Hernieuwingsdatum.Hydrate(hernieuwingsdatum),
            HernieuwingsUrl = HernieuwingsUrl.Hydrate(hernieuwingsUrl),
            RedenSchorsing = motivering,
            Status = ErkenningStatus.Hydrate(status),
        };

    public bool HeeftConflictMet(Erkenning teRegistrerenErkenning)
    {
        var zelfdeSleutel =
            IpdcProduct.Nummer == teRegistrerenErkenning.IpdcProduct.Nummer
            && GeregistreerdDoor.OvoCode == teRegistrerenErkenning.GeregistreerdDoor.OvoCode;

        if (!zelfdeSleutel)
            return false;

        return ErkenningsPeriode.OverlapsWith(teRegistrerenErkenning.ErkenningsPeriode);
    }

    public Erkenning Schors(string redenSchorsing) =>
        this with
        {
            RedenSchorsing = redenSchorsing,
            Status = ErkenningStatus.Geschorst,
        };

    public Erkenning CreateFromErkenningWijziging(ErkenningWijziging erkenningWijziging)
    {
        return this with
        {
            ErkenningsPeriode = erkenningWijziging.ErkenningsPeriode,
            Hernieuwingsdatum = erkenningWijziging.Hernieuwingsdatum,
            HernieuwingsUrl = erkenningWijziging.HernieuwingsUrl,
            Status = erkenningWijziging.Status,
        };
    }

    public bool KanGeactiveerdWordenOp(DateOnly today)
    {
        if (Status != ErkenningStatus.InAanvraag)
            return false;

        if (!ErkenningsPeriode.Startdatum.HasValue)
            return false;

        return ErkenningStatus.Bepaal(ErkenningsPeriode, today) == ErkenningStatus.Actief;
    }

    public bool KanVerlopenWordenOp(DateOnly today)
    {
        if (Status != ErkenningStatus.Actief)
            return false;

        if (!ErkenningsPeriode.Einddatum.HasValue)
            return false;

        return ErkenningStatus.Bepaal(ErkenningsPeriode, today) == ErkenningStatus.Verlopen;
    }
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
