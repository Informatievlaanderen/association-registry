namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using Events;
using JasperFx.Events;
using Marten.Events;
using Resources;
using Zoeken;
using IEvent = JasperFx.Events.IEvent;

public class BeheerZoekenEventsConsumer : IMartenEventsConsumer
{
    private readonly BeheerZoekProjectionHandler _zoekProjectionHandler;
    private readonly ILogger<BeheerZoekenEventsConsumer> _logger;

    public BeheerZoekenEventsConsumer(BeheerZoekProjectionHandler zoekProjectionHandler, ILogger<BeheerZoekenEventsConsumer> logger)
    {
        _zoekProjectionHandler = zoekProjectionHandler;
        _logger = logger;
    }

    public async Task ConsumeAsync(IReadOnlyList<IEvent> events)
    {
        foreach (var @event in events)
        {
            dynamic eventEnvelope =
                Activator.CreateInstance(typeof(EventEnvelope<>).MakeGenericType(@event.EventType), @event)!;

            switch (@event.EventType.Name)
            {
                case nameof(DoelgroepWerdGewijzigd):
                case nameof(FeitelijkeVerenigingWerdGeregistreerd):
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
                        await _zoekProjectionHandler.Handle(eventEnvelope);

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
