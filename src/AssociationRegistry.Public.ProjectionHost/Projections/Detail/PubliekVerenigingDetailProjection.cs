﻿namespace AssociationRegistry.Public.ProjectionHost.Projections.Detail;

using Events;
using Infrastructure.Extensions;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Schema.Detail;

public class PubliekVerenigingDetailProjection : EventProjection
{
    public PubliekVerenigingDetailProjection()
    {
        // Needs a batch size of 1, because otherwise if Registered and NameChanged arrive in 1 batch/slice,
        // the newly persisted document from xxxWerdGeregistreerd is not in the
        // Query yet when we handle NaamWerdGewijzigd.
        // see also https://martendb.io/events/projections/event-projections.html#reusing-documents-in-the-same-batch
        Options.BatchSize = 1;
        Options.DeleteViewTypeOnTeardown<PubliekVerenigingDetailDocument>();
    }

    public void Project(IEvent<FeitelijkeVerenigingWerdGeregistreerd> @event, IDocumentOperations ops)
        => Create(@event, ops, PubliekVerenigingDetailProjector.Create);

    public void Project(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> @event,
        IDocumentOperations ops)
        => Create(@event, ops, PubliekVerenigingDetailProjector.Create);

    public async Task Project(IEvent<NaamWerdGewijzigd> @event, IDocumentOperations ops)
    {
        var updateDocs = Enumerable.Empty<PubliekVerenigingDetailDocument>().ToList();
        var vereniging = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(@event.StreamKey!))!;

        var gerelateerdeVerenigingen = ops.Query<PubliekVerenigingDetailDocument>()
                                          .Where(d => d.Relaties.Any(r => r.AndereVereniging.VCode == vereniging.VCode))
                                          .ToList();

        foreach (var gerelateerdeVereniging in gerelateerdeVerenigingen)
        {
            gerelateerdeVereniging.Relaties = gerelateerdeVereniging.Relaties
                                                                    .UpdateSingle(
                                                                         identityFunc: relatie
                                                                             => relatie.AndereVereniging.VCode == @event.Data.VCode,
                                                                         update: r =>
                                                                         {
                                                                             r.AndereVereniging.Naam = @event.Data.Naam;

                                                                             return r;
                                                                         })
                                                                    .ToArray();

            PubliekVerenigingDetailProjector.UpdateMetadata(@event, gerelateerdeVereniging);
            updateDocs.Add(gerelateerdeVereniging);
        }

        PubliekVerenigingDetailProjector.Apply(@event, vereniging);
        PubliekVerenigingDetailProjector.UpdateMetadata(@event, vereniging);

        updateDocs.Add(vereniging);
        ops.StoreObjects(updateDocs);
    }

    public async Task Project(IEvent<RoepnaamWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<KorteNaamWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<KorteBeschrijvingWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<StartdatumWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<DoelgroepWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdToegevoegd> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdVerwijderd> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<LocatieWerdToegevoegd> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<LocatieWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<LocatieWerdVerwijderd> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(
        IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(
        IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(
        IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(
        IEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdOvergenomenUitKBO> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<ContactgegevenUitKBOWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public async Task Project(IEvent<VerenigingWerdGestopt> @event, IDocumentOperations ops)
        => await Update(@event, ops, PubliekVerenigingDetailProjector.Apply);

    public void Project(IEvent<VerenigingWerdVerwijderd> @event, IDocumentOperations ops)
        => ops.Delete<PubliekVerenigingDetailDocument>(@event.StreamKey);

    private static async Task Update<T>(
        IEvent<T> @event,
        IDocumentOperations ops,
        Action<IEvent<T>, PubliekVerenigingDetailDocument> action) where T : notnull
        => await Update(@event.StreamKey!, @event, ops, action);

    private static async Task Update<T>(
        string vCode,
        IEvent<T> @event,
        IDocumentOperations ops,
        Action<IEvent<T>, PubliekVerenigingDetailDocument> action) where T : notnull
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(vCode))!;

        action(@event, doc);
        PubliekVerenigingDetailProjector.UpdateMetadata(@event, doc);

        ops.Store(doc);
    }

    private static void Create<T>(
        IEvent<T> @event,
        IDocumentOperations ops,
        Func<IEvent<T>, PubliekVerenigingDetailDocument> action) where T : notnull
    {
        var doc = action(@event);
        PubliekVerenigingDetailProjector.UpdateMetadata(@event, doc);
        ops.Insert(doc);
    }
}
