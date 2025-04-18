namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Events;
using Marten.Events;
using Resources;
using IEvent = Marten.Events.IEvent;

public class MartenEventsConsumer : IMartenEventsConsumer
{
    private readonly PubliekZoekProjectionHandler _handler;
    private readonly ILogger<MartenEventsConsumer> _logger;

    public MartenEventsConsumer(PubliekZoekProjectionHandler handler, ILogger<MartenEventsConsumer> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task ConsumeAsync(IReadOnlyList<StreamAction> streamActions)
    {
        foreach (var @event in streamActions.SelectMany(streamAction => streamAction.Events))
        {
            dynamic eventEnvelope =
                Activator.CreateInstance(typeof(EventEnvelope<>).MakeGenericType(@event.EventType), @event)!;

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
                    try
                    {
                        await _handler.Handle(eventEnvelope);

                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ExceptionMessages.FoutBijProjecteren);

                        throw;
                    }
            }
        }
    }
}

public class EventEnvelope<T> : IEventEnvelope
{
    public long Sequence
        => Event.Sequence;

    public string VCode
        => Event.StreamKey!;

    public T Data
        => (T)Event.Data;

    public Dictionary<string, object>? Headers
        => Event.Headers;

    public EventEnvelope(IEvent @event)
    {
        Event = @event;
    }

    private IEvent Event { get; }
}

public interface IEventEnvelope
{
}
