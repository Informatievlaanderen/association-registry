namespace AssociationRegistry.KboMutations.SyncLambda.Services;

using DecentraalBeheer.Vereniging;
using Integrations.Slack;
using MagdaSync.SyncKbo;
using MagdaSync.SyncKsz;
using MartenDb.Store;
using Microsoft.Extensions.Logging;

public record LambdaServices(
    MessageProcessor MessageProcessor,
    ILoggerFactory LoggerFactory,
    SyncKboCommandHandler KboSyncHandler,
    SyncKszMessageHandler KszSyncHandler,
    AggregateSession Repository,
    VerenigingStateQueryService QueryService,
    INotifier Notifier
) { }
