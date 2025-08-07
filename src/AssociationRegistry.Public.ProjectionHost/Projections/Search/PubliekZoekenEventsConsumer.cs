namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.MGet;
using Events;
using Hosts.Configuration.ConfigurationBindings;
using MartenDb.Subscriptions;
using Resources;
using Schema.Search;

public class PubliekZoekenEventsConsumer : IMartenEventsConsumer
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly PubliekZoekProjectionHandler _zoekProjectionHandler;
    private readonly ElasticSearchOptionsSection _options;
    private readonly ILogger<PubliekZoekenEventsConsumer> _logger;

    public PubliekZoekenEventsConsumer(ElasticsearchClient elasticClient,
                                       PubliekZoekProjectionHandler zoekProjectionHandler,
                                       ElasticSearchOptionsSection options,
                                       ILogger<PubliekZoekenEventsConsumer> logger)
    {
        _elasticClient = elasticClient;
        _zoekProjectionHandler = zoekProjectionHandler;
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
            Index = _options.Indices.Verenigingen,
            Docs = keys.Select(key => new MultiGetOperation
            {
                Id = key
            }).ToList()
        };

        var multiGetResponse = await _elasticClient.MultiGetAsync<VerenigingZoekDocument>(multiGetRequest);

        var documentsPerVCode = new Dictionary<string, VerenigingZoekDocument>();

        foreach (var vCode in eventList.GroupedByVCode.Keys)
        {
            var hit = multiGetResponse.Docs.FirstOrDefault(h => h.Value1?.Id == vCode);

            if (hit != null && hit.Value1 != null && hit.Value1.Source is VerenigingZoekDocument document)
            {
                documentsPerVCode.Add(vCode, document);
            }
            else
            {
                documentsPerVCode.Add(vCode, new VerenigingZoekDocument { VCode = vCode });
            }
        }

        foreach (var @event in eventList.Events)
        {
            dynamic eventEnvelope =
                Activator.CreateInstance(typeof(EventEnvelope<>).MakeGenericType(@event.EventType), @event)!;

            var doc = documentsPerVCode[@event.StreamKey];

            switch (@event.EventType.Name)
            {
                case nameof(FeitelijkeVerenigingWerdGeregistreerd):
                case nameof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd):
                case nameof(FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid):
                case nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd):
                case nameof(NaamWerdGewijzigd):
                case nameof(RoepnaamWerdGewijzigd):
                case nameof(KorteNaamWerdGewijzigd):
                case nameof(KorteBeschrijvingWerdGewijzigd):
                case nameof(DoelgroepWerdGewijzigd):
                case nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd):
                case nameof(WerkingsgebiedenWerdenNietBepaald):
                case nameof(WerkingsgebiedenWerdenBepaald):
                case nameof(WerkingsgebiedenWerdenGewijzigd):
                case nameof(WerkingsgebiedenWerdenNietVanToepassing):
                case nameof(VerenigingWerdUitgeschrevenUitPubliekeDatastroom):
                case nameof(VerenigingWerdIngeschrevenInPubliekeDatastroom):
                case nameof(LocatieWerdToegevoegd):
                case nameof(LocatieWerdGewijzigd):
                case nameof(LocatieWerdVerwijderd):
                case nameof(MaatschappelijkeZetelWerdOvergenomenUitKbo):
                case nameof(MaatschappelijkeZetelVolgensKBOWerdGewijzigd):
                case nameof(MaatschappelijkeZetelWerdGewijzigdInKbo):
                case nameof(MaatschappelijkeZetelWerdVerwijderdUitKbo):
                case nameof(VerenigingWerdGestopt):
                case nameof(VerenigingWerdGestoptInKBO):
                case nameof(VerenigingWerdVerwijderd):
                case nameof(NaamWerdGewijzigdInKbo):
                case nameof(KorteNaamWerdGewijzigdInKbo):
                case nameof(RechtsvormWerdGewijzigdInKBO):
                case nameof(AdresWerdOvergenomenUitAdressenregister):
                case nameof(AdresWerdGewijzigdInAdressenregister):
                case nameof(LocatieDuplicaatWerdVerwijderdNaAdresMatch):
                case nameof(LidmaatschapWerdToegevoegd):
                case nameof(LidmaatschapWerdGewijzigd):
                case nameof(LidmaatschapWerdVerwijderd):
                case nameof(VerenigingWerdGemarkeerdAlsDubbelVan):
                case nameof(WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt):
                case nameof(MarkeringDubbeleVerengingWerdGecorrigeerd):
                case nameof(VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging):
                case nameof(VerenigingssubtypeWerdTerugGezetNaarNietBepaald):
                case nameof(VerenigingssubtypeWerdVerfijndNaarSubvereniging):
                case nameof(SubverenigingRelatieWerdGewijzigd):
                case nameof(SubverenigingDetailsWerdenGewijzigd):
                case nameof(GeotagsWerdenBepaald):
                    try
                    {
                        _zoekProjectionHandler.Handle(eventEnvelope, doc);

                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, string.Format(ExceptionMessages.FoutBijProjecteren, ProjectionNames.PubliekZoek));

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
                                                          .Index(_options.Indices.Verenigingen)
                                                          .IndexMany(documents)
                                                          .Refresh(Refresh.WaitFor)
        );

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

