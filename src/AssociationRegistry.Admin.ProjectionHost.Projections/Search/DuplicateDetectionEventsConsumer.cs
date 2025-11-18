namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using DuplicateDetection;
using Events;
using Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.MartenDb.Subscriptions;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.MGet;
using Events.Enriched;
using Microsoft.Extensions.Logging;
using Resources;
using Schema.Search;

public class DuplicateDetectionEventsConsumer : IMartenEventsConsumer
{

    private readonly ElasticsearchClient _elasticClient;
    private readonly DuplicateDetectionProjectionHandler _duplicateDetectionProjectionHandler;
    private readonly ElasticSearchOptionsSection _options;
    private readonly ILogger<DuplicateDetectionEventsConsumer> _logger;

    public DuplicateDetectionEventsConsumer(
        ElasticsearchClient elasticClient,
        DuplicateDetectionProjectionHandler duplicateDetectionProjectionHandler,
        ElasticSearchOptionsSection options,
        ILogger<DuplicateDetectionEventsConsumer> logger)
    {
        _elasticClient = elasticClient;
        _duplicateDetectionProjectionHandler = duplicateDetectionProjectionHandler;
        _options = options;
        _logger = logger;
    }

    public async Task ConsumeAsync(SubscriptionEventList eventList)
    {
        if (!eventList.Events.Any())
            return;

        var keys = eventList.GroupedByVCode.Keys;

        var multiGetRequest = new MultiGetRequest
        {
            Index = _options.Indices.DuplicateDetection,
            Docs = keys.Select(key => new MultiGetOperation
            {
                Id = key
            }).ToList()
        };

        var multiGetResponse = await _elasticClient.MultiGetAsync<DuplicateDetectionDocument>(multiGetRequest);

        var documentsPerVCode = new Dictionary<string, DuplicateDetectionDocument>();

        foreach (var vCode in eventList.GroupedByVCode.Keys)
        {
            var hit = multiGetResponse.Docs.FirstOrDefault(h => h.Value1?.Id == vCode);

            if (hit != null && hit.Value1 != null && hit.Value1.Source is DuplicateDetectionDocument document)
            {
                documentsPerVCode.Add(vCode, document);
            }
            else
            {
                documentsPerVCode.Add(vCode, new DuplicateDetectionDocument() { VCode = vCode });
            }
        }

        foreach (var @event in eventList.Events)
        {
            dynamic eventEnvelope =
                Activator.CreateInstance(typeof(EventEnvelope<>).MakeGenericType(@event.EventType), @event)!;

            var doc = documentsPerVCode[@event.StreamKey];

            switch (@event.EventType.Name)
            {
                case nameof(FeitelijkeVerenigingWerdGeregistreerdMetPersoonsgegevens):
                case nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens):
                case nameof(FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid):
                case nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd):
                case nameof(KorteNaamWerdGewijzigd):
                case nameof(LocatieWerdGewijzigd):
                case nameof(LocatieWerdToegevoegd):
                case nameof(LocatieWerdVerwijderd):
                case nameof(MaatschappelijkeZetelWerdOvergenomenUitKbo):
                case nameof(MaatschappelijkeZetelWerdGewijzigdInKbo):
                case nameof(MaatschappelijkeZetelWerdVerwijderdUitKbo):
                case nameof(NaamWerdGewijzigd):
                case nameof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd):
                case nameof(VerenigingWerdGestopt):
                case nameof(VerenigingWerdGestoptInKBO):
                case nameof(VerenigingWerdVerwijderd):
                case nameof(NaamWerdGewijzigdInKbo):
                case nameof(KorteNaamWerdGewijzigdInKbo):
                case nameof(AdresWerdOvergenomenUitAdressenregister):
                case nameof(AdresWerdGewijzigdInAdressenregister):
                case nameof(LocatieDuplicaatWerdVerwijderdNaAdresMatch):
                case nameof(VerenigingWerdGemarkeerdAlsDubbelVan):
                case nameof(WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt):
                case nameof(MarkeringDubbeleVerengingWerdGecorrigeerd):
                case nameof(VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging):
                case nameof(VerenigingssubtypeWerdTerugGezetNaarNietBepaald):
                case nameof(VerenigingssubtypeWerdVerfijndNaarSubvereniging):
                case nameof(MaatschappelijkeZetelVolgensKBOWerdGewijzigd):
                    try
                    {
                        _duplicateDetectionProjectionHandler.Handle(eventEnvelope, doc);

                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, string.Format(ExceptionMessages.FoutBijProjecteren, ProjectionNames.DuplicateDetection));

                        throw;
                    }
            }
        }
        var bulkAll = await IndexDocumentsAsync(documentsPerVCode.Values);

    }

    private async Task<bool> IndexDocumentsAsync<T>(IEnumerable<T> documents)
        where T : class
    {
        var response = await _elasticClient.BulkAsync(b => b
                                                          .Index(_options.Indices.DuplicateDetection)
                                                          .IndexMany(documents)
                                                          .Refresh(Refresh.WaitFor));

        if (!response.IsValidResponse)
        {
            foreach (var item in response.Items.Where(i => i.Error is not null))
            {
                _logger.LogError("Failed to index document {Id}: {Error}", item.Id, item.Error?.Reason);
            }

            return false;
        }

        _logger.LogInformation($"Successfully indexed {response.Items.Count} documents");

        return true;
    }
}
