namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using EventStore;

public class CommandResult
{
    private CommandResult(VCode vcode, long? sequence, long? version)
    {
        Vcode = vcode;
        Sequence = sequence;
        Version = version;
    }

    public VCode Vcode { get; }
    public long? Sequence { get; }
    public long? Version { get; }

    public bool HasChanges()
        => Sequence is not null;

    public static CommandResult Create(VCode vCode, StreamActionResult streamActionResult)
        => new(vCode, streamActionResult.Sequence, streamActionResult.Version);
}
