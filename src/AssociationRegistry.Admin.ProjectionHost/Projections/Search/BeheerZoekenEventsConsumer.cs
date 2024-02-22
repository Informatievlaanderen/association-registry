namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using Events;
using Marten.Events;
using Zoeken;

public class BeheerZoekenEventsConsumer : IMartenEventsConsumer
{
    private readonly BeheerZoekProjectionHandler _zoekProjectionHandler;

    public BeheerZoekenEventsConsumer(BeheerZoekProjectionHandler zoekProjectionHandler)
    {
        _zoekProjectionHandler = zoekProjectionHandler;
    }

    public async Task ConsumeAsync(IReadOnlyList<StreamAction> streamActions)
    {
        foreach (var @event in streamActions.SelectMany(streamAction => streamAction.Events))
        {
            dynamic eventEnvelope =
                Activator.CreateInstance(typeof(EventEnvelope<>).MakeGenericType(@event.EventType), @event)!;

            switch (@event.EventType.Name)
            {
                case nameof(DoelgroepWerdGewijzigd):
                case nameof(FeitelijkeVerenigingWerdGeregistreerd):
                case nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd):
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
                    await _zoekProjectionHandler.Handle(eventEnvelope);
                    break;
            }
        }
    }
}
