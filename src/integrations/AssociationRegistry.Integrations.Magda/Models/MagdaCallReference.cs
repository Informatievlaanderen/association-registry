namespace AssociationRegistry.Integrations.Magda.Models;

using Marten.Schema;

public class MagdaCallReference
{
    [Identity]
    public Guid Reference { get; set; }
    public string Initiator { get; set; } = null!;
    public string OpgevraagdeDienst { get; set; } = null!;
    public string OpgevraagdOnderwerp { get; set; } = null!;
    public Guid CorrelationId { get; set; }
    public string Context { get; set; } = null!;
    public string AanroependeDienst { get; set; } = null!;
    public DateTimeOffset CalledAt { get; set; }
}
