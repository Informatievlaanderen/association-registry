namespace AssociationRegistry.EventStore;

public record StreamActionResult(long? Sequence, long? Version)
{
    public static StreamActionResult Empty
        => new(Sequence: null, Version: null);
}
