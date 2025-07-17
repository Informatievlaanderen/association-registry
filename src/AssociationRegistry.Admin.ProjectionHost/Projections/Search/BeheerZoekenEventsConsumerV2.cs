namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using Elasticsearch.Net;
using Events;
using JasperFx.Core;
using JasperFx.Events;
using Marten.Events;
using Nest;
using Resources;
using Schema.Search;
using Zoeken;
using IEvent = JasperFx.Events.IEvent;

public class BeheerZoekenEventsConsumerV2 : IMartenEventsConsumer
{
    private readonly IElasticClient _elasticClient;
    private readonly BeheerZoekProjectionHandlerV2 _zoekProjectionHandler;
    private readonly ILogger<BeheerZoekenEventsConsumerV2> _logger;

    public BeheerZoekenEventsConsumerV2(
        IElasticClient elasticClient,
        BeheerZoekProjectionHandlerV2 zoekProjectionHandler,
        ILogger<BeheerZoekenEventsConsumerV2> logger)
    {
        _elasticClient = elasticClient;
        _zoekProjectionHandler = zoekProjectionHandler;
        _logger = logger;
    }

    public async Task ConsumeAsync(IReadOnlyList<IEvent> events)
    {
        var eventsPerVCode = events.GroupBy(x => x.StreamKey)
                                   .ToDictionary(x => x.Key, x => x.ToList());

        var ids = eventsPerVCode.Keys.Select(k => (Id)k);

        var searchResponse = await _elasticClient
           .SearchAsync<VerenigingZoekDocument>(s => s
                                                   .Query(q => q
                                                             .Ids(i => i
                                                                     .Values(ids)
                                                              )
                                                    )
            );

        var documentsPerVCode = new Dictionary<string, VerenigingZoekDocument>();
        foreach (var vCode in eventsPerVCode.Keys)
        {
            if (searchResponse.Documents.Any(x => x.VCode == vCode))
            {
                documentsPerVCode.Add(vCode, searchResponse.Documents.Single(x => x.VCode == vCode));
            }
            else
            {
                documentsPerVCode.Add(vCode, new VerenigingZoekDocument());
            }
        }


        foreach (var @event in events)
        {
            dynamic eventEnvelope =
                Activator.CreateInstance(typeof(EventEnvelope<>).MakeGenericType(@event.EventType), @event)!;

            var doc = documentsPerVCode[@event.StreamKey];

            // if(!cache.contains(event.streamkey)
            // var document = new ElasticRepository().LoadORNull(event.Streamkey) ?? new Document
            // cache.Add(doc);
            // else
            // doc = cache.get(streamkey);

            switch (@event.EventType.Name)
            {
                case nameof(FeitelijkeVerenigingWerdGeregistreerd):
                    //
                    // case nameof(DoelgroepWerdGewijzigd):
                    case nameof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd):
                    // case nameof(FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid):
                    // case nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd):
                    // case nameof(WerkingsgebiedenWerdenNietBepaald):
                    case nameof(WerkingsgebiedenWerdenBepaald):
                    // case nameof(WerkingsgebiedenWerdenGewijzigd):
                    // case nameof(WerkingsgebiedenWerdenNietVanToepassing):
                    // case nameof(KorteNaamWerdGewijzigd):
                    // case nameof(LocatieWerdGewijzigd):
                    // case nameof(LocatieWerdToegevoegd):
                    // case nameof(LocatieWerdVerwijderd):
                    // case nameof(MaatschappelijkeZetelVolgensKBOWerdGewijzigd):
                    // case nameof(MaatschappelijkeZetelWerdOvergenomenUitKbo):
                    // case nameof(NaamWerdGewijzigd):
                    // case nameof(RoepnaamWerdGewijzigd):
                    // case nameof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd):
                    // case nameof(VerenigingWerdGestopt):
                    // case nameof(VerenigingWerdIngeschrevenInPubliekeDatastroom):
                    // case nameof(VerenigingWerdUitgeschrevenUitPubliekeDatastroom):
                    // case nameof(VerenigingWerdVerwijderd):
                    // case nameof(NaamWerdGewijzigdInKbo):
                    // case nameof(KorteNaamWerdGewijzigdInKbo):
                    // case nameof(MaatschappelijkeZetelWerdGewijzigdInKbo):
                    // case nameof(MaatschappelijkeZetelWerdVerwijderdUitKbo):
                    // case nameof(RechtsvormWerdGewijzigdInKBO):
                    // case nameof(VerenigingWerdGestoptInKBO):
                    // case nameof(StartdatumWerdGewijzigd):
                    // case nameof(StartdatumWerdGewijzigdInKbo):
                    // case nameof(EinddatumWerdGewijzigd):
                    // case nameof(AdresWerdOvergenomenUitAdressenregister):
                    // case nameof(AdresWerdGewijzigdInAdressenregister):
                    // case nameof(LocatieDuplicaatWerdVerwijderdNaAdresMatch):
                    // case nameof(LidmaatschapWerdToegevoegd):
                    // case nameof(LidmaatschapWerdGewijzigd):
                    // case nameof(LidmaatschapWerdVerwijderd):
                    // case nameof(VerenigingWerdGemarkeerdAlsDubbelVan):
                    // case nameof(VerenigingAanvaarddeDubbeleVereniging):
                    // case nameof(WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt):
                    // case nameof(MarkeringDubbeleVerengingWerdGecorrigeerd):
                    // case nameof(VerenigingAanvaarddeCorrectieDubbeleVereniging):
                    // case nameof(VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging):
                    // case nameof(VerenigingssubtypeWerdTerugGezetNaarNietBepaald):
                    // case nameof(VerenigingssubtypeWerdVerfijndNaarSubvereniging):
                    // case nameof(SubverenigingRelatieWerdGewijzigd):
                    // case nameof(SubverenigingDetailsWerdenGewijzigd):
                    case nameof(GeotagsWerdenBepaald):
                    try
                    {
                        _zoekProjectionHandler.Handle(eventEnvelope, doc);

                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ExceptionMessages.FoutBijProjecteren);

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
                                                  .IndexMany(documents)
                                                  .Refresh(Refresh.WaitFor) // Makes docs immediately searchable
        );
        if (!response.IsValid)
        {
            // Handle errors
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
