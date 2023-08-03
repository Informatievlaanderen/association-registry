namespace AssociationRegistry.Magda.Models;

using System;

public class MagdaCallReference
{
    public Guid Reference { get; set; }
    public string Initiator { get; set; }
    public string OpgevraagdeDienst { get; set; }
    public string OpgevraagdOnderwerp { get; set; }
    public Guid CorrelationId { get; set; }
    public string Context { get; set; }
    public string AanroependeDienst { get; set; }
    public DateTimeOffset CalledAt { get; set; }
}
