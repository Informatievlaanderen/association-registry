namespace AssociationRegistry.KboMutations.SyncLambda.Services;

using Integrations.Magda.Onderneming;
using Integrations.Slack;
using MagdaSync.SyncKsz.Queries;
using MartenDb.Store;
using Microsoft.Extensions.Logging;
using Persoonsgegevens;

public record LambdaServices(
    MessageProcessor MessageProcessor,
    ILoggerFactory LoggerFactory,
    MagdaRegistreerInschrijvingService RegistreerInschrijvingService,
    SyncGeefVerenigingService GeefVerenigingService,
    VerenigingsRepository Repository,
    FilterVzerOnlyQuery FilterVzerOnlyQuery,
    IVertegenwoordigerPersoonsgegevensRepository VertegenwoordigerPersoonsgegevensRepository,
    INotifier Notifier);
