namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record TeRegistrerenErkenning
{
    public string IpdcProductNummer { get; set; } = null!;
    public ErkenningsPeriode ErkenningsPeriode { get; set; } = null!;
    public HernieuwingsUrl HernieuwingsUrl { get; set; } = null!;
    public Hernieuwingsdatum Hernieuwingsdatum { get; set; } = null!;
}
