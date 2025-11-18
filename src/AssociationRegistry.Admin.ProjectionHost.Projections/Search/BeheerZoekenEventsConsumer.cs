namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using Events;
using Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.MartenDb.Subscriptions;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.MGet;
using Events.Enriched;
using Microsoft.Extensions.Logging;
using Resources;
using Schema.Search;
using Zoeken;

public class BeheerZoekenEventsConsumer : IMartenEventsConsumer
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly BeheerZoekProjectionHandler _zoekProjectionHandler;
    private readonly ElasticSearchOptionsSection _options;
    private readonly ILogger<BeheerZoekenEventsConsumer> _logger;

    public BeheerZoekenEventsConsumer(
        ElasticsearchClient elasticClient,
        BeheerZoekProjectionHandler zoekProjectionHandler,
        ElasticSearchOptionsSection options,
        ILogger<BeheerZoekenEventsConsumer> logger)
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

        foreach (var vCode in keys)
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
                case nameof(FeitelijkeVerenigingWerdGeregistreerdMetPersoonsgegevens):
                case nameof(DoelgroepWerdGewijzigd):
                case nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens):
                case nameof(FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid):
                case nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd):
                case nameof(WerkingsgebiedenWerdenNietBepaald):
                case nameof(WerkingsgebiedenWerdenBepaald):
                case nameof(WerkingsgebiedenWerdenGewijzigd):
                case nameof(WerkingsgebiedenWerdenNietVanToepassing):
                case nameof(KorteNaamWerdGewijzigd):
                case nameof(LocatieWerdGewijzigd):
                case nameof(LocatieWerdToegevoegd):
                case nameof(LocatieWerdVerwijderd):
                case nameof(MaatschappelijkeZetelVolgensKBOWerdGewijzigd):
                case nameof(MaatschappelijkeZetelWerdOvergenomenUitKbo):
                case nameof(NaamWerdGewijzigd):
                case nameof(RoepnaamWerdGewijzigd):
                case nameof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd):
                case nameof(VerenigingWerdGestopt):
                case nameof(VerenigingWerdIngeschrevenInPubliekeDatastroom):
                case nameof(VerenigingWerdUitgeschrevenUitPubliekeDatastroom):
                case nameof(VerenigingWerdVerwijderd):
                case nameof(NaamWerdGewijzigdInKbo):
                case nameof(KorteNaamWerdGewijzigdInKbo):
                case nameof(MaatschappelijkeZetelWerdGewijzigdInKbo):
                case nameof(MaatschappelijkeZetelWerdVerwijderdUitKbo):
                case nameof(RechtsvormWerdGewijzigdInKBO):
                case nameof(VerenigingWerdGestoptInKBO):
                case nameof(StartdatumWerdGewijzigd):
                case nameof(StartdatumWerdGewijzigdInKbo):
                case nameof(EinddatumWerdGewijzigd):
                case nameof(AdresWerdOvergenomenUitAdressenregister):
                case nameof(AdresWerdGewijzigdInAdressenregister):
                case nameof(LocatieDuplicaatWerdVerwijderdNaAdresMatch):
                case nameof(LidmaatschapWerdToegevoegd):
                case nameof(LidmaatschapWerdGewijzigd):
                case nameof(LidmaatschapWerdVerwijderd):
                case nameof(VerenigingWerdGemarkeerdAlsDubbelVan):
                case nameof(VerenigingAanvaarddeDubbeleVereniging):
                case nameof(WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt):
                case nameof(MarkeringDubbeleVerengingWerdGecorrigeerd):
                case nameof(VerenigingAanvaarddeCorrectieDubbeleVereniging):
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
                        _logger.LogError(ex, string.Format(ExceptionMessages.FoutBijProjecteren, ProjectionNames.BeheerZoek));

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
            var errorItems = response.Items.Where(i => i.Error is not null);

            foreach (var item in errorItems)
            {
                _logger.LogError("Failed to index document {Id}: {Error}", item.Id, item.Error?.Reason);
            }

            throw new BulkAllFailed(errorItems.Select(x => x.Error.Reason).ToArray(), response.DebugInformation);
        }

        _logger.LogInformation($"Successfully indexed {response.Items.Count} documents");

        return true;
    }
}
