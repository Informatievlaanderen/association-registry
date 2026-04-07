namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public class TeRegistrerenErkenning()
{
    public string IpdcProductNummer { get; set; } = null!;
    public DateOnly Startdatum { get; set; }
    public DateOnly Einddatum { get; set; }
    public DateOnly Hernieuwingsdatum { get; set; }
    public string HernieuwingsUrl { get; set; } = null!;
}
