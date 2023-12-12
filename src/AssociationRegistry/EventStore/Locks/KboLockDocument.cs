namespace AssociationRegistry.EventStore.Locks;

using Marten.Schema;

public class KboLockDocument
{
    [Identity]
    public string KboNummer { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
