namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record TeRegistrerenErkenning
{
    public IpdcProduct IpdcProduct { get; set; } = null!;
    public DateOnly Startdatum { get; set; }
    public DateOnly Einddatum { get; set; }
    public DateOnly Hernieuwingsdatum { get; set; }
    public string HernieuwingsUrl { get; set; } = null!;
}
