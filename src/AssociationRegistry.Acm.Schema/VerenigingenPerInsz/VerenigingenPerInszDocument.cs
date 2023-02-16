namespace AssociationRegistry.Acm.Schema.VerenigingenPerInsz;

using Marten.Schema;

public class VerenigingenPerInszDocument
{
    [Identity] public string Insz { get; set; } = null!;
    public List<Vereniging> Verenigingen { get; set; } = new();
}

public class Vereniging
{
    public string VCode { get; set; }
    public string Naam { get; set; }
}
