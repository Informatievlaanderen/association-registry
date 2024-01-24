namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using DuplicateDetection;
using Events;
using Marten.Events;
using Wolverine;
using Wolverine.Runtime.Routing;
using Zoeken;
using IEvent = Framework.IEvent;

public class MartenEventsConsumer : IMartenEventsConsumer
{
    private readonly IMessageBus _bus;
    private readonly DuplicateDetectionProjectionHandler _duplicateDetectionProjectionHandler;
    private readonly BeheerZoekProjectionHandler _zoekProjectionHandler;

    public MartenEventsConsumer(IMessageBus bus,
                                DuplicateDetectionProjectionHandler duplicateDetectionProjectionHandler,
                                BeheerZoekProjectionHandler zoekProjectionHandler)
    {
        _bus = bus;
        _duplicateDetectionProjectionHandler = duplicateDetectionProjectionHandler;
        _zoekProjectionHandler = zoekProjectionHandler;
    }

    public async Task ConsumeAsync(IReadOnlyList<StreamAction> streamActions)
    {
        foreach (var @event in streamActions.SelectMany(streamAction => streamAction.Events))
        {
            var eventEnvelope =
                (IEventEnvelope)Activator.CreateInstance(typeof(EventEnvelope<>).MakeGenericType(@event.EventType), @event)!;

            switch (@event.EventType.Name)
            {

                case nameof(DoelgroepWerdGewijzigd):
                    await _zoekProjectionHandler.Handle((EventEnvelope<DoelgroepWerdGewijzigd>)eventEnvelope);
                    break;

                case nameof(FeitelijkeVerenigingWerdGeregistreerd):
                    await _duplicateDetectionProjectionHandler.Handle((EventEnvelope<FeitelijkeVerenigingWerdGeregistreerd>)eventEnvelope);
                    await _zoekProjectionHandler.Handle((EventEnvelope<FeitelijkeVerenigingWerdGeregistreerd>)eventEnvelope);
                    break;

                case nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd):
                    await _duplicateDetectionProjectionHandler.Handle((EventEnvelope<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>)eventEnvelope);
                    await _zoekProjectionHandler.Handle((EventEnvelope<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>)eventEnvelope);
                    break;

                case nameof(KorteNaamWerdGewijzigd):
                    await _duplicateDetectionProjectionHandler.Handle((EventEnvelope<KorteNaamWerdGewijzigd>)eventEnvelope);
                    await _zoekProjectionHandler.Handle((EventEnvelope<KorteNaamWerdGewijzigd>)eventEnvelope);
                    break;

                case nameof(LocatieWerdGewijzigd):
                    await _duplicateDetectionProjectionHandler.Handle((EventEnvelope<LocatieWerdGewijzigd>)eventEnvelope);
                    await _zoekProjectionHandler.Handle((EventEnvelope<LocatieWerdGewijzigd>)eventEnvelope);
                    break;

                case nameof(LocatieWerdToegevoegd):
                    await _duplicateDetectionProjectionHandler.Handle((EventEnvelope<LocatieWerdToegevoegd>)eventEnvelope);
                    await _zoekProjectionHandler.Handle((EventEnvelope<LocatieWerdToegevoegd>)eventEnvelope);
                    break;

                case nameof(LocatieWerdVerwijderd):
                    await _duplicateDetectionProjectionHandler.Handle((EventEnvelope<LocatieWerdVerwijderd>)eventEnvelope);
                    await _zoekProjectionHandler.Handle((EventEnvelope<LocatieWerdVerwijderd>)eventEnvelope);
                    break;


                case nameof(MaatschappelijkeZetelVolgensKBOWerdGewijzigd):
                    await _zoekProjectionHandler.Handle((EventEnvelope<MaatschappelijkeZetelVolgensKBOWerdGewijzigd>)eventEnvelope);
                    break;

                case nameof(MaatschappelijkeZetelWerdOvergenomenUitKbo):
                    await _duplicateDetectionProjectionHandler.Handle((EventEnvelope<MaatschappelijkeZetelWerdOvergenomenUitKbo>)eventEnvelope);
                    await _zoekProjectionHandler.Handle((EventEnvelope<MaatschappelijkeZetelWerdOvergenomenUitKbo>)eventEnvelope);
                    break;

                case nameof(NaamWerdGewijzigd):
                    await _duplicateDetectionProjectionHandler.Handle((EventEnvelope<NaamWerdGewijzigd>)eventEnvelope);
                    await _zoekProjectionHandler.Handle((EventEnvelope<NaamWerdGewijzigd>)eventEnvelope);
                    break;

                case nameof(RoepnaamWerdGewijzigd):
                    await _zoekProjectionHandler.Handle((EventEnvelope<RoepnaamWerdGewijzigd>)eventEnvelope);
                    break;


                case nameof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd):
                    await _duplicateDetectionProjectionHandler.Handle((EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>)eventEnvelope);
                    await _zoekProjectionHandler.Handle((EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>)eventEnvelope);
                    break;

                case nameof(VerenigingWerdGestopt):
                    await _duplicateDetectionProjectionHandler.Handle((EventEnvelope<VerenigingWerdGestopt>)eventEnvelope);
                    await _zoekProjectionHandler.Handle((EventEnvelope<VerenigingWerdGestopt>)eventEnvelope);
                    break;

                case nameof(VerenigingWerdIngeschrevenInPubliekeDatastroom):
                    await _zoekProjectionHandler.Handle((EventEnvelope<VerenigingWerdIngeschrevenInPubliekeDatastroom>)eventEnvelope);
                    break;

                case nameof(VerenigingWerdUitgeschrevenUitPubliekeDatastroom):
                    await _zoekProjectionHandler.Handle((EventEnvelope<VerenigingWerdUitgeschrevenUitPubliekeDatastroom>)eventEnvelope);
                    break;

                case nameof(VerenigingWerdVerwijderd):
                    await _duplicateDetectionProjectionHandler.Handle((EventEnvelope<VerenigingWerdVerwijderd>)eventEnvelope);
                    await _zoekProjectionHandler.Handle((EventEnvelope<VerenigingWerdVerwijderd>)eventEnvelope);
                    break;
            }
        }
    }
}
