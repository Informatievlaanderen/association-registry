namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Elasticsearch.Net;
using Events;
using Hosts.Configuration.ConfigurationBindings;
using Nest;
using Resources;
using Schema.Search;
using IEvent = JasperFx.Events.IEvent;

public class PubliekZoekenEventsConsumer : IMartenEventsConsumer
{
    private readonly IElasticClient _elasticClient;
    private readonly PubliekZoekProjectionHandler _zoekProjectionHandler;
    private readonly ElasticSearchOptionsSection _options;
    private readonly ILogger<PubliekZoekenEventsConsumer> _logger;

    public PubliekZoekenEventsConsumer(IElasticClient elasticClient,
                                       PubliekZoekProjectionHandler zoekProjectionHandler,
                                       ElasticSearchOptionsSection options,
                                       ILogger<PubliekZoekenEventsConsumer> logger)
    {
        _elasticClient = elasticClient;
        _zoekProjectionHandler = zoekProjectionHandler;
        _options = options;
        _logger = logger;
    }

    public async Task ConsumeAsync(IReadOnlyList<IEvent> events)
    {
        var eventsPerVCode = events.GroupBy(x => x.StreamKey)
                                   .ToDictionary(x => x.Key, x => x.ToList());

        var multiGetResponse = await _elasticClient
           .MultiGetAsync(m => m
                              .Index(_options.Indices.Verenigingen)
                              .GetMany<VerenigingZoekDocument>(eventsPerVCode.Keys)
            );

        var documentsPerVCode = new Dictionary<string, VerenigingZoekDocument>();

        foreach (var vCode in eventsPerVCode.Keys)
        {
            var hit = multiGetResponse.Hits.FirstOrDefault(h => h.Id == vCode);

            if (hit != null && hit.Found && hit.Source is VerenigingZoekDocument document)
            {
                documentsPerVCode.Add(vCode, document);
            }
            else
            {
                documentsPerVCode.Add(vCode, new VerenigingZoekDocument { VCode = vCode });
            }
        }

        foreach (var @event in events)
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

        if (!response.IsValid && !response.ApiCall.Success)
        {
            foreach (var error in response.ItemsWithErrors.Where(x => x.Error != null))
            {
                _logger.LogError($"Failed to index document {error.Id}: {error.Error}");
            }

            return false;
        }

        _logger.LogInformation($"Successfully indexed {response.Items.Count} documents");

        return true;
    }
}
