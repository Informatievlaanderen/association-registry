namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Events;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Schema.Detail;

public class BeheerVerenigingDetailProjection : EventProjection
{
    public BeheerVerenigingDetailProjection()
    {
        // Needs a batch size of 1, because otherwise if Registered and NameChanged arrive in 1 batch/slice,
        // the newly persisted document from xxxWerdGeregistreerd is not in the
        // Query yet when we handle NaamWerdGewijzigd.
        // see also https://martendb.io/events/projections/event-projections.html#reusing-documents-in-the-same-batch

        Options.EnableDocumentTrackingByIdentity = true;
        Options.DeleteViewTypeOnTeardown<BeheerVerenigingDetailDocument>();
    }

    public BeheerVerenigingDetailDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> @event, IDocumentOperations ops)
        => DoCreate(@event, ops, BeheerVerenigingDetailProjector.Create);

    public BeheerVerenigingDetailDocument Create(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> @event,
        IDocumentOperations ops)
        => DoCreate(@event, ops, BeheerVerenigingDetailProjector.Create);

    public async Task Project(IEvent<NaamWerdGewijzigd> @event, IDocumentOperations ops)
    {
        var updateDocs = Enumerable.Empty<BeheerVerenigingDetailDocument>().ToList();
        var vereniging = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(@event.StreamKey!))!;

        var gerelateerdeVerenigingen = ops.Query<BeheerVerenigingDetailDocument>()
                                          .Where(d => d.Relaties.Any(r => r.AndereVereniging.VCode == vereniging.VCode))
                                          .ToList();

        foreach (var gerelateerdeVereniging in gerelateerdeVerenigingen)
        {
            gerelateerdeVereniging.Relaties = gerelateerdeVereniging.Relaties.UpdateSingleWith(
                                                                         identityFunc: relatie
                                                                             => relatie.AndereVereniging.VCode == @event.Data.VCode,
                                                                         update: r => r with
                                                                         {
                                                                             AndereVereniging = r.AndereVereniging with
                                                                             {
                                                                                 Naam = @event.Data.Naam,
                                                                             },
                                                                         })
                                                                    .ToArray();

            BeheerVerenigingDetailProjector.UpdateMetadata(@event, gerelateerdeVereniging);
            updateDocs.Add(gerelateerdeVereniging);
        }

        BeheerVerenigingDetailProjector.Apply(@event, vereniging);
        BeheerVerenigingDetailProjector.UpdateMetadata(@event, vereniging);
        updateDocs.Add(vereniging);
        ops.StoreObjects(updateDocs);
    }

    public async Task Project(IEvent<KorteNaamWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<KorteBeschrijvingWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<StartdatumWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<StartdatumWerdGewijzigdInKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<DoelgroepWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdToegevoegd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdVerwijderd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<WerkingsgebiedenWerdenNietBepaald> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<WerkingsgebiedenWerdenBepaald> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<WerkingsgebiedenWerdenGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<WerkingsgebiedenWerdenNietVanToepassing> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<VertegenwoordigerWerdToegevoegd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<VertegenwoordigerWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<VertegenwoordigerWerdVerwijderd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<LocatieWerdToegevoegd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<LocatieWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<LocatieWerdVerwijderd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdOvergenomenUitKBO> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<RoepnaamWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<VerenigingWerdGestopt> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<EinddatumWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<VertegenwoordigerWerdOvergenomenUitKBO> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenUitKBOWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<VerenigingWerdVerwijderd> @event, IDocumentOperations ops)
        => await SoftDelete(@event.StreamKey, ops);

    public async Task Project(IEvent<NaamWerdGewijzigdInKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<KorteNaamWerdGewijzigdInKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdGewijzigdInKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdVerwijderdUitKBO> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdInBeheerGenomenDoorKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<MaatschappelijkeZetelWerdGewijzigdInKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<RechtsvormWerdGewijzigdInKBO> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<VerenigingWerdGestoptInKBO> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<AdresWerdOvergenomenUitAdressenregister> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<AdresWerdNietGevondenInAdressenregister> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<AdresNietUniekInAdressenregister> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<AdresKonNietOvergenomenWordenUitAdressenregister> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<AdresWerdGewijzigdInAdressenregister> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<AdresWerdOntkoppeldVanAdressenregister> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<SynchronisatieMetKboWasSuccesvol> @event, IDocumentOperations ops)
        => await UpdateMetadataOnly(@event, ops);

    public async Task Project(IEvent<ContactgegevenKonNietOvergenomenWordenUitKBO> @event, IDocumentOperations ops)
        => await UpdateMetadataOnly(@event, ops);

    public async Task Project(IEvent<MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo> @event, IDocumentOperations ops)
        => await UpdateMetadataOnly(@event, ops);

    public async Task Project(IEvent<VerenigingWerdIngeschrevenOpWijzigingenUitKbo> @event, IDocumentOperations ops)
        => await UpdateMetadataOnly(@event, ops);

    public async Task Project(IEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<AdresHeeftGeenVerschillenMetAdressenregister> @event, IDocumentOperations ops)
        => await UpdateMetadataOnly(@event, ops);

    public async Task Project(IEvent<LidmaatschapWerdToegevoegd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<LidmaatschapWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<LidmaatschapWerdVerwijderd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<VerenigingWerdGermarkeerdAlsDubbelVan> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingDetailProjector.Apply);

    private async Task SoftDelete(string? streamKey, IDocumentOperations ops)
        => ops.Delete<BeheerVerenigingDetailDocument>(streamKey);

    private static async Task Update<T>(
        IEvent<T> @event,
        IDocumentOperations ops,
        Action<IEvent<T>, BeheerVerenigingDetailDocument> action) where T : notnull
        => await Update(@event.StreamKey!, @event, ops, action);

    private static async Task Update<T>(
        string vCode,
        IEvent<T> @event,
        IDocumentOperations ops,
        Action<IEvent<T>, BeheerVerenigingDetailDocument> action) where T : notnull
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(vCode))!;

        action(@event, doc);
        BeheerVerenigingDetailProjector.UpdateMetadata(@event, doc);

        ops.Store(doc);
    }

    private static async Task UpdateMetadataOnly<T>(
        IEvent<T> @event,
        IDocumentOperations ops) where T : notnull
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(@event.StreamKey))!;

        BeheerVerenigingDetailProjector.UpdateMetadata(@event, doc);

        ops.Store(doc);
    }

    private static BeheerVerenigingDetailDocument DoCreate<T>(
        IEvent<T> @event,
        IDocumentOperations ops,
        Func<IEvent<T>, BeheerVerenigingDetailDocument> action) where T : notnull
    {
        var doc = action(@event);
        BeheerVerenigingDetailProjector.UpdateMetadata(@event, doc);
        ops.Insert(doc);

        return doc;
    }
}
