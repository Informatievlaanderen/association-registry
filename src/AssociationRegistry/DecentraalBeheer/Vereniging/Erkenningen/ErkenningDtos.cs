namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

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
