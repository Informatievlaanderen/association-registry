namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record Erkenning
{
    public int ErkenningId { get; set; }
    public string VCode { get; set; } = null!;
    public GegevensInitiator[] GeregistreerdDoor { get; set; } = [];
    public IpdcProduct IpdcProduct { get; set; } = null!;
    public DateOnly Startdatum { get; set; }
    public DateOnly Einddatum { get; set; }
    public DateOnly Hernieuwingsdatum { get; set; }
    public string HernieuwingsUrl { get; set; } = null!;
    public string Motivering { get; set; } = null!;

    public static Erkenning Create(int nextId, TeRegistrerenErkenning erkenning) =>
        new()
        {
            ErkenningId = nextId,
            Startdatum = erkenning.Startdatum,
            Einddatum = erkenning.Einddatum,
            Hernieuwingsdatum = erkenning.Hernieuwingsdatum,
            HernieuwingsUrl = erkenning.HernieuwingsUrl,
        };

    public static Erkenning Hydrate(
        int id,
        string vCode,
        GegevensInitiator[] geregistreerdDoor,
        IpdcProduct ipdcProduct,
        DateOnly startdatum,
        DateOnly einddatum,
        DateOnly hernieuwingsdatum,
        string hernieuwingsUrl,
        string motivering) =>
        new()
        {
            ErkenningId = id,
            VCode = vCode,
            GeregistreerdDoor = geregistreerdDoor,
            IpdcProduct = ipdcProduct,
            Startdatum = startdatum,
            Einddatum = einddatum,
            Hernieuwingsdatum = hernieuwingsdatum,
            HernieuwingsUrl = hernieuwingsUrl,
            Motivering = motivering
        };
}

public class IpdcProduct
{
    public string Nummer { get; set; }
    public string Naam { get; set; }
}

public class GegevensInitiator
{
    public string OvoCode { get; set; }
    public string Naam { get; set; }
}
