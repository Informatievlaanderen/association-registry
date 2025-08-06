namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using EventStore;

public class EntityCommandResult
{
    private EntityCommandResult(VCode vcode, int entityId, long? sequence, long? version)
    {
        Vcode = vcode;
        EntityId = entityId;
        Sequence = sequence;
        Version = version;
    }

    public VCode Vcode { get; }
    public int EntityId { get; set; }
    public long? Sequence { get; }
    public long? Version { get; }

    public bool HasChanges()
        => Sequence is not null;

    public static EntityCommandResult Create(VCode vCode, int entityId, StreamActionResult streamActionResult)
        => new(vCode, entityId, streamActionResult.Sequence, streamActionResult.Version);
}
