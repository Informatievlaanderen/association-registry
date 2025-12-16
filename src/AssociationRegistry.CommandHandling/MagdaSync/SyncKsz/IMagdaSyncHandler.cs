namespace AssociationRegistry.CommandHandling.MagdaSync.SyncKsz;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;

public interface IMagdaSyncHandler
{
    bool CanHandle(string body);
    Task<CommandResult> Handle(
        string body,
        CommandMetadata commandMetadata,
        CancellationToken cancellationToken);
}

public interface IMagdaSyncHandler<TCommand> : IMagdaSyncHandler
{
    Task<CommandResult> Handle(
        CommandEnvelope<TCommand> envelope,
        CancellationToken cancellationToken);
}
