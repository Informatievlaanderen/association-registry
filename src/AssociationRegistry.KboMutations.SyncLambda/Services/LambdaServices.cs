namespace AssociationRegistry.KboMutations.SyncLambda.Services;

using AssociationRegistry.EventStore;
using AssociationRegistry.Integrations.Magda;
using CommandHandling.Magda;
using Integrations.Magda.GeefOnderneming;
using Integrations.Slack;
using MartenDb.Store;
using Microsoft.Extensions.Logging;

public record LambdaServices(
    MessageProcessor MessageProcessor,
    ILoggerFactory LoggerFactory,
    MagdaRegistreerInschrijvingService RegistreerInschrijvingService,
    SyncGeefVerenigingService GeefVerenigingService,
    VerenigingsRepository Repository,
    INotifier Notifier
);
