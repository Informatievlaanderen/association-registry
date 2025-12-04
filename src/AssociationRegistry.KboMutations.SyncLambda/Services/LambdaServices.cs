namespace AssociationRegistry.KboMutations.SyncLambda.Services;

using Integrations.Magda.Onderneming;
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
