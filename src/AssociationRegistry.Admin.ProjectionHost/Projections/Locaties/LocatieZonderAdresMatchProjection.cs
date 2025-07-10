namespace AssociationRegistry.Admin.ProjectionHost.Projections.Locaties;

using Events;
using Grar.Clients;
using Marten.Events.Aggregation;
using Schema.Detail;
using Vereniging;

public class LocatieZonderAdresMatchProjection : SingleStreamProjection<LocatieZonderAdresMatchDocument, string>
{
    public LocatieZonderAdresMatchProjection(ILogger<LocatieZonderAdresMatchProjection> logger)
    {
        Options.BatchSize = 1;
        Options.EnableDocumentTrackingByIdentity = true;
        Options.MaximumHopperSize = 1;
        Options.DeleteViewTypeOnTeardown<LocatieZonderAdresMatchDocument>();

        CreateEvent<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>(e => new LocatieZonderAdresMatchDocument()
        {
            VCode = e.VCode,
            LocatieIds = e.Locaties.Select(x => x.LocatieId).ToArray(),
        });

        ProjectEvent<AdresWerdOvergenomenUitAdressenregister>((doc, e) => LocatieZonderAdresId(doc, e.LocatieId));

        DeleteEvent<VerenigingWerdVerwijderd>();
        ProjectEvent<LocatieWerdToegevoegd>((doc, @event) =>
        {
            if (@event.Locatie.Locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde) return doc;

            if (@event.Locatie.Adres is null) return doc;

            return doc with
            {
                LocatieIds = doc.LocatieIds.Append(@event.Locatie.LocatieId).ToArray()
            };
        });
        ProjectEvent<LocatieWerdGewijzigd>((doc, @event) =>
        {
            if (@event.Locatie.Locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde) return doc;

            if (@event.Locatie.Adres is null) return doc;

            if(doc.LocatieIds.Contains(@event.Locatie.LocatieId)) return doc;

            return doc with
            {
                LocatieIds = doc.LocatieIds.Append(@event.Locatie.LocatieId).ToArray(),
            };
        });

        ProjectEvent<LocatieWerdVerwijderd>((doc, e) => LocatieZonderAdresId(doc, e.Locatie.LocatieId));
        ProjectEvent<AdresWerdOvergenomenUitAdressenregister>((doc, e) => LocatieZonderAdresId(doc, e.LocatieId));
        ProjectEvent<AdresWerdNietGevondenInAdressenregister>((doc, e) => LocatieZonderAdresId(doc, e.LocatieId));
        ProjectEvent<AdresNietUniekInAdressenregister>((doc, e) => LocatieZonderAdresId(doc, e.LocatieId));
        ProjectEvent<AdresWerdOntkoppeldVanAdressenregister>((doc, e) => LocatieZonderAdresId(doc, e.LocatieId));
        ProjectEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch>((doc, e) => LocatieZonderAdresId(doc, e.VerwijderdeLocatieId));
        ProjectEvent<AdresHeeftGeenVerschillenMetAdressenregister>((doc, e) => LocatieZonderAdresId(doc, e.LocatieId));

        ProjectEvent<AdresKonNietOvergenomenWordenUitAdressenregister>((doc, e) => e.Reden switch
        {
            GrarClient.BadRequestSuccessStatusCodeMessage or
                AdresKonNietOvergenomenWordenUitAdressenregister.RedenLocatieWerdVerwijderd
                => LocatieZonderAdresId(doc, e.LocatieId),
            _ => doc,
        });
    }

    private static LocatieZonderAdresMatchDocument LocatieZonderAdresId(LocatieZonderAdresMatchDocument doc, int locatieId)
    {
        return doc with {LocatieIds =
            doc.LocatieIds.Where(x => x != locatieId).ToArray()};
    }
}
