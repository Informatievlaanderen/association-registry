namespace AssociationRegistry.Vereniging;

using EventStore;
using VCodes;

public class CommandResult
{
    private CommandResult(VCode vcode, long? sequence, long? version)
    {
        Vcode = vcode;
        Sequence = sequence;
        Version = version;
    }

    public bool HasChanges()
        => Sequence is not null;

    public VCode Vcode { get; }
    public long? Sequence { get; }
    public long? Version { get; }

    public static CommandResult Create(VCode vCode, StreamActionResult streamActionResult)
        => new(vCode, streamActionResult.Sequence, streamActionResult.Version);
}
