namespace AssociationRegistry.KboMutations.SyncLambda.Services;

using Integrations.Magda.Onderneming;
using Integrations.Slack;
using MartenDb.Store;
using Microsoft.Extensions.Logging;
using Persoonsgegevens;

public record LambdaServices(
    MessageProcessor MessageProcessor,
    ILoggerFactory LoggerFactory,
    MagdaRegistreerInschrijvingService RegistreerInschrijvingService,
    SyncGeefVerenigingService GeefVerenigingService,
    VerenigingsRepository Repository,
    IVertegenwoordigerPersoonsgegevensRepository VertegenwoordigerPersoonsgegevensRepository,
    INotifier Notifier
);
