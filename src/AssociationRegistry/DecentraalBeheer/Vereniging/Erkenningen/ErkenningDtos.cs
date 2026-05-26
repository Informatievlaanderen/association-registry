namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using Exceptions;
using Primitives;

public record TeRegistrerenErkenning
{
    public string IpdcProductNummer { get; set; } = null!;
    public ErkenningsPeriode ErkenningsPeriode { get; set; } = null!;
    public HernieuwingsUrl HernieuwingsUrl { get; set; } = null!;
    public Hernieuwingsdatum Hernieuwingsdatum { get; set; } = null!;
}

public record TeSchorsenErkenning
{
    public int ErkenningId { get; set; }
    public string RedenSchorsing { get; set; } = null!;
}

public record TeCorrigerenSchorsingErkenning
{
    public int ErkenningId { get; set; }
    public string RedenSchorsing { get; set; } = null!;
}

public record TeCorrigerenErkenning
{
    private TeCorrigerenErkenning(int erkenningId, NullOrEmpty<DateOnly> startDatum, NullOrEmpty<DateOnly> eindDatum, NullOrEmpty<DateOnly> hernieuwingsDatum, string? hernieuwingsUrl, string wijzigingsType)
    {
        ErkenningId = erkenningId;
        StartDatum = startDatum;
        EindDatum = eindDatum;
        Hernieuwingsdatum = hernieuwingsDatum;
        HernieuwingsUrl = hernieuwingsUrl;
        WijzigingsType = wijzigingsType;
    }

    public static TeCorrigerenErkenning Create(
        int erkenningId,
        NullOrEmpty<DateOnly> startDatum,
        NullOrEmpty<DateOnly> eindDatum,
        NullOrEmpty<DateOnly> hernieuwingsDatum,
        string? hernieuwingsUrl,
        string type)
    {
        if (HeeftGeenTeWijzigenWaarde(startDatum, eindDatum, hernieuwingsDatum, hernieuwingsUrl))
            throw new TeCorrigerenErkenningMoetMinstensEenTeCorrigerenWaardeHebben();

        if (string.IsNullOrEmpty(type))
            throw new TeCorrigerenErkenningMoetMinstensEenTeCorrigerenWaardeHebben();

        return new TeCorrigerenErkenning(erkenningId, startDatum, eindDatum, hernieuwingsDatum, hernieuwingsUrl, type);
    }

    public int ErkenningId { get; set; }
    public NullOrEmpty<DateOnly> StartDatum { get; set; }
    public NullOrEmpty<DateOnly> EindDatum { get; set; }
    public string? HernieuwingsUrl { get; set; } = null!;
    public NullOrEmpty<DateOnly> Hernieuwingsdatum { get; set; }
    public string WijzigingsType { get; set; }

    public static bool HeeftGeenTeWijzigenWaarde(
        NullOrEmpty<DateOnly> startDatum,
        NullOrEmpty<DateOnly> eindDatum,
        NullOrEmpty<DateOnly> hernieuwingsDatum,
        string? hernieuwingsUrl)
        => startDatum.IsNull &&
           eindDatum.IsNull &&
           hernieuwingsUrl is null &&
           hernieuwingsDatum.IsNull;
}
