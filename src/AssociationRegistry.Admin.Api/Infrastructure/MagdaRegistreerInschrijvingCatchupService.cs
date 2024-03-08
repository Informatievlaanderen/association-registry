namespace AssociationRegistry.Admin.Api.Infrastructure;

using Amazon.SQS;
using ConfigurationBindings;
using Events;
using Kbo;
using Marten;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Vereniging;

public class MagdaRegistreerInschrijvingCatchupService : IMagdaRegistreerInschrijvingCatchupService
{
    private readonly AppSettings _appSettings;
    private readonly IDocumentStore _documentStore;
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<MagdaRegistreerInschrijvingCatchupService> _logger;

    public MagdaRegistreerInschrijvingCatchupService(
        AppSettings appSettings,
        IDocumentStore documentStore,
        IAmazonSQS sqsClient,
        ILogger<MagdaRegistreerInschrijvingCatchupService> logger)
    {
        _appSettings = appSettings;
        _documentStore = documentStore;
        _sqsClient = sqsClient;
        _logger = logger;
    }

    public async Task RegistreerInschrijvingVoorVerenigingenMetRechtspersoonlijkheidDieNogNietIngeschrevenZijn()
    {
        var kboNummers = await GetKboNummersZonderRegistreerInschrijving();

        _logger.LogWarning($"MAGDA RegistreerInschrijving Catchup Service : {kboNummers.Count} KBO nummers gevonden");

        foreach (var kboNummer in kboNummers)
        {
            await _sqsClient.SendMessageAsync(
                _appSettings.KboSyncQueueUrl,
                JsonSerializer.Serialize(
                    new TeSynchroniserenKboNummerMessage(kboNummer.Value)));

            _logger.LogInformation(
                $"MAGDA RegistreerInschrijving Catchup Service : KBO nummer {kboNummer} werd op de sync queue geplaatst");
        }
    }

    public async Task<IReadOnlyCollection<KboNummer>> GetKboNummersZonderRegistreerInschrijving()
    {
        await using var session = _documentStore.LightweightSession();

        var alleVerenigingenGeregistreerd = session.Events
                                                   .QueryAllRawEvents()
                                                   .OfType<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                                                   .Select(e => KboNummer.Create(e.KboNummer))
                                                   .Distinct()
                                                   .ToHashSet();

        var alleVerenigingenInschrijvingGeregistreerd = session.Events
                                                               .QueryAllRawEvents()
                                                               .OfType<VerenigingWerdIngeschrevenOpWijzigingenUitKbo>()
                                                               .Select(e => KboNummer.Create(e.KboNummer))
                                                               .Distinct()
                                                               .ToHashSet();

        alleVerenigingenGeregistreerd.RemoveWhere(v => alleVerenigingenInschrijvingGeregistreerd.Contains(v));

        return alleVerenigingenGeregistreerd.ToArray();
    }
}
