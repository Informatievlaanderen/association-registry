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
        Options.BatchSize = 1;
    }

    public void Project(IEvent<FeitelijkeVerenigingWerdGeregistreerd> @event, IDocumentOperations ops)
        => Create(@event, ops, BeheerVerenigingDetailProjector.Create);

    public void Project(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> @event,
        IDocumentOperations ops)
        => Create(@event, ops, BeheerVerenigingDetailProjector.Create);

    public async Task Project(IEvent<AfdelingWerdGeregistreerd> @event, IDocumentOperations ops)
    {
        Create(@event, ops, BeheerVerenigingDetailProjector.Create);

        if (!string.IsNullOrEmpty(@event.Data.Moedervereniging.VCode))
            await Update(@event.Data.Moedervereniging.VCode, @event, ops,
                         BeheerVerenigingDetailProjector.Apply);
    }

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

    private static void Create<T>(
        IEvent<T> @event,
        IDocumentOperations ops,
        Func<IEvent<T>, BeheerVerenigingDetailDocument> action) where T : notnull
    {
        var doc = action(@event);
        BeheerVerenigingDetailProjector.UpdateMetadata(@event, doc);
        ops.Insert(doc);
    }
}
