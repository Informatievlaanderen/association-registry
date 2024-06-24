namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Events;
using Marten.Events;
using Wolverine;
using IEvent = Marten.Events.IEvent;

public class MartenEventsConsumer : IMartenEventsConsumer
{
    private readonly IMessageBus _bus;
    private readonly ILogger<MartenEventsConsumer> _logger;

    public MartenEventsConsumer(IMessageBus bus, ILogger<MartenEventsConsumer> logger)
    {
        _bus = bus;
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
                case nameof(NaamWerdGewijzigd):
                case nameof(RoepnaamWerdGewijzigd):
                case nameof(KorteNaamWerdGewijzigd):
                case nameof(KorteBeschrijvingWerdGewijzigd):
                case nameof(DoelgroepWerdGewijzigd):
                case nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd):
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
                    try
                    {
                        await _bus.InvokeAsync(eventEnvelope);

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

