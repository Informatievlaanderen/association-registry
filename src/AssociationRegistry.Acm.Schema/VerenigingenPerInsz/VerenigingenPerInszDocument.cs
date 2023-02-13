namespace AssociationRegistry.Acm.Schema.VerenigingenPerInsz;

using Marten.Schema;

public class VerenigingenPerInszDocument
{
    [Identity] public string Insz { get; set; } = null!;
    public Vereniging[] Verenigingen { get; set; } = Array.Empty<Vereniging>();
}

public record Vereniging(
    string VCode,
    string Naam);
