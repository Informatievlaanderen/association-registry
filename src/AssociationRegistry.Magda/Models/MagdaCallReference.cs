namespace AssociationRegistry.Magda.Models;

using System;

public class MagdaCallReference
{
    public Guid Reference { get; set; }
    public string? Initiator { get; set; }
    public DateTimeOffset CalledAt { get; set; }
}
