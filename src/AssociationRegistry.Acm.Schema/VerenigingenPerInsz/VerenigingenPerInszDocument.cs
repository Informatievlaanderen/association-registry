namespace AssociationRegistry.Acm.Schema.VerenigingenPerInsz;

using Marten.Schema;

public class VerenigingenPerInszDocument
{
    [Identity] public string Insz { get; set; } = null!;
    public List<Vereniging> Verenigingen { get; set; } = new();
}

public class Vereniging
{
    public Vereniging(
        string VCode,
        string Naam)
    {
        this.VCode = VCode;
        this.Naam = Naam;
    }

    public string VCode { get; set; }
    public string Naam { get; set; }
}
