namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using Elasticsearch.Net;
using Events;
using Hosts.Configuration.ConfigurationBindings;
using Nest;
using Resources;
using Schema.Search;
using Zoeken;
using IEvent = JasperFx.Events.IEvent;

public class BeheerZoekenEventsConsumer : IMartenEventsConsumer
{
    private readonly IElasticClient _elasticClient;
    private readonly BeheerZoekProjectionHandler _zoekProjectionHandler;
    private readonly ElasticSearchOptionsSection _options;
    private readonly ILogger<BeheerZoekenEventsConsumer> _logger;

    public BeheerZoekenEventsConsumer(
        IElasticClient elasticClient,
        BeheerZoekProjectionHandler zoekProjectionHandler,
        ElasticSearchOptionsSection options,
        ILogger<BeheerZoekenEventsConsumer> logger)
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
                case nameof(DoelgroepWerdGewijzigd):
                case nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd):
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

    public async Task<bool> IndexDocumentsAsync<T>(IEnumerable<T> documents)
        where T : class
    {
        var response = await _elasticClient.BulkAsync(b => b
                                                          .Index(_options.Indices.Verenigingen)
                                                          .IndexMany(documents)
                                                          .Refresh(Refresh.WaitFor)
        );

        if (!response.IsValid)
        {
            foreach (var error in response.ItemsWithErrors)
            {
                Console.WriteLine($"Failed to index document {error.Id}: {error.Error}");
            }

            return false;
        }

        Console.WriteLine($"Successfully indexed {response.Items.Count} documents");

        return true;
    }
}
