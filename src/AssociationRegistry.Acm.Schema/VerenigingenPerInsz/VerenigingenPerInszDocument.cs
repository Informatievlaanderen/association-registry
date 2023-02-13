namespace AssociationRegistry.Acm.Schema.VerenigingenPerInsz;

using Marten.Schema;

public class VerenigingenPerInszDocument
{
    [Identity] public string Insz { get; set; } = null!;
    public Vereniging[] Verenigingen { get; set; } = Array.Empty<Vereniging>();
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
