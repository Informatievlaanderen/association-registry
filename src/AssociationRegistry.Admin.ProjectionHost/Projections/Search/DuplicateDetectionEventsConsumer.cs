namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using DuplicateDetection;
using Events;
using IEvent = JasperFx.Events.IEvent;

public class DuplicateDetectionEventsConsumer : IMartenEventsConsumer
{
    private readonly DuplicateDetectionProjectionHandler _duplicateDetectionProjectionHandler;

    public DuplicateDetectionEventsConsumer(DuplicateDetectionProjectionHandler duplicateDetectionProjectionHandler)
    {
        _duplicateDetectionProjectionHandler = duplicateDetectionProjectionHandler;
    }

    public async Task ConsumeAsync(IReadOnlyList<IEvent> events)
    {
        foreach (var @event in events)
        {
            dynamic eventEnvelope =
                (IEventEnvelope)Activator.CreateInstance(typeof(EventEnvelope<>).MakeGenericType(@event.EventType), @event)!;

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
                    await _duplicateDetectionProjectionHandler.Handle(eventEnvelope);

                    break;
            }
        }
    }
}
