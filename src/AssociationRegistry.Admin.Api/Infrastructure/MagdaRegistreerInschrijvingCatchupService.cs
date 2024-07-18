namespace AssociationRegistry.Admin.Api.Infrastructure;

using AWS;
using Events;
using Hosts.Configuration.ConfigurationBindings;
using Kbo;
using Marten;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MagdaRegistreerInschrijvingCatchupService : IMagdaRegistreerInschrijvingCatchupService
{
    private readonly AppSettings _appSettings;
    private readonly IDocumentStore _documentStore;
    private readonly SqsClientWrapper _sqsClient;
    private readonly ILogger<MagdaRegistreerInschrijvingCatchupService> _logger;

    public MagdaRegistreerInschrijvingCatchupService(
        SqsClientWrapper sqsClient,
        IDocumentStore documentStore,
        ILogger<MagdaRegistreerInschrijvingCatchupService> logger)
    {
        _documentStore = documentStore;
        _sqsClient = sqsClient;
        _logger = logger;
    }

    public async Task RegistreerInschrijvingVoorVerenigingenMetRechtspersoonlijkheidDieNogNietIngeschrevenZijn()
    {
        try
        {
            var kboNummers = await GetKboNummersZonderRegistreerInschrijving();

            _logger.LogWarning($"MAGDA RegistreerInschrijving Catchup Service : {kboNummers.Count} KBO nummers gevonden");

            foreach (var kboNummer in kboNummers)
            {
                await _sqsClient.QueueKboNummerToSynchronise(kboNummer);

                _logger.LogInformation(
                    $"MAGDA RegistreerInschrijving Catchup Service : KBO nummer {kboNummer} werd op de sync queue geplaatst");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"MAGDA RegistreerInschrijving Catchup Service : {ex.Message}");
        }
    }

    public async Task<IReadOnlyCollection<string>> GetKboNummersZonderRegistreerInschrijving()
    {
        await using var session = _documentStore.LightweightSession();

        var alleVerenigingenGeregistreerd = session.Events
                                                   .QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                                                   .Select(e => e.KboNummer)
                                                   .ToArray();

        var alleVerenigingenInschrijvingGeregistreerd = session.Events
                                                               .QueryRawEventDataOnly<VerenigingWerdIngeschrevenOpWijzigingenUitKbo>()
                                                               .Select(e => e.KboNummer)
                                                               .ToArray();

        return alleVerenigingenGeregistreerd.Except(alleVerenigingenInschrijvingGeregistreerd).ToArray();
    }
}
