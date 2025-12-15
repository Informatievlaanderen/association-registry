namespace AssociationRegistry.CommandHandling.MagdaSync.SyncKsz;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;

public interface IMagdaSyncHandler
{
    Task<CommandResult> Handle<TCommand>(
        CommandEnvelope<TCommand> envelope,
        CancellationToken cancellationToken);
}
