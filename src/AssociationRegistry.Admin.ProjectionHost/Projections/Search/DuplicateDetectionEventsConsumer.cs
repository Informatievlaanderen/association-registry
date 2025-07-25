namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using DuplicateDetection;
using Elasticsearch.Net;
using Events;
using Hosts.Configuration.ConfigurationBindings;
using Nest;
using Resources;
using Schema.Search;
using IEvent = JasperFx.Events.IEvent;

public class DuplicateDetectionEventsConsumer : IMartenEventsConsumer
{

    private readonly IElasticClient _elasticClient;
    private readonly DuplicateDetectionProjectionHandler _duplicateDetectionProjectionHandler;
    private readonly ElasticSearchOptionsSection _options;
    private readonly ILogger<DuplicateDetectionEventsConsumer> _logger;

    public DuplicateDetectionEventsConsumer(
        IElasticClient elasticClient,
        DuplicateDetectionProjectionHandler duplicateDetectionProjectionHandler,
        ElasticSearchOptionsSection options,
        ILogger<DuplicateDetectionEventsConsumer> logger)
    {
        _elasticClient = elasticClient;
        _duplicateDetectionProjectionHandler = duplicateDetectionProjectionHandler;
        _options = options;
        _logger = logger;
    }

    public async Task ConsumeAsync(IReadOnlyList<IEvent> events)
    {
        var eventsPerVCode = events.GroupBy(x => x.StreamKey)
                                   .ToDictionary(x => x.Key, x => x.ToList());

        var ids = eventsPerVCode.Keys.Select(k => (Id)k);

        var multiGetResponse = await _elasticClient
           .MultiGetAsync(m => m
                              .Index(_options.Indices.DuplicateDetection)
                              .GetMany<DuplicateDetectionDocument>(eventsPerVCode.Keys)
            );

        var documentsPerVCode = new Dictionary<string, DuplicateDetectionDocument>();

        foreach (var vCode in eventsPerVCode.Keys)
        {
            // Use the Hits collection and check the Source property
            var hit = multiGetResponse.Hits.FirstOrDefault(h => h.Id == vCode);

            if (hit != null && hit.Found && hit.Source is DuplicateDetectionDocument document)
            {
                documentsPerVCode.Add(vCode, document);
            }
            else
            {
                documentsPerVCode.Add(vCode, new DuplicateDetectionDocument { VCode = vCode });
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
                case nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd):
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

    public async Task<bool> IndexDocumentsAsync<T>(IEnumerable<T> documents)
        where T : class
    {
        var response = await _elasticClient.BulkAsync(b =>
            {
                return b
                      .Index(_options.Indices.DuplicateDetection)
                      .IndexMany(documents)
                      .Refresh(Refresh.WaitFor);
            }
            // Makes docs immediately searchable
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
