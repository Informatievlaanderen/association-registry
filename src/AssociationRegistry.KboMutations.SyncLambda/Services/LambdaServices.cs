namespace AssociationRegistry.KboMutations.SyncLambda.Services;

using AssociationRegistry.EventStore;
using AssociationRegistry.Magda;
using Integrations.Slack;
using MartenDb.Store;
using Microsoft.Extensions.Logging;

public record LambdaServices(
    MessageProcessor MessageProcessor,
    ILoggerFactory LoggerFactory,
    MagdaRegistreerInschrijvingService RegistreerInschrijvingService,
    MagdaGeefVerenigingService GeefOndernemingService,
    VerenigingsRepository Repository,
    INotifier Notifier
);
