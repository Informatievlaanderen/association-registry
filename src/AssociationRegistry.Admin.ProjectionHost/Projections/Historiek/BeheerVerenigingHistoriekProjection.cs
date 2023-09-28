namespace AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;

using System.Threading.Tasks;
using Events;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Schema.Historiek;

public class BeheerVerenigingHistoriekProjection : EventProjection
{
    public BeheerVerenigingHistoriekProjection()
    {
        // Needs a batch size of 1, because otherwise if Registered and NameChanged arrive in 1 batch/slice,
        // the newly persisted document from xxxWerdGeregistreerd is not in the
        // Query yet when we handle NaamWerdGewijzigd.
        // see also https://martendb.io/events/projections/event-projections.html#reusing-documents-in-the-same-batch
        Options.BatchSize = 1;
    }

    public void Project(
        IEvent<FeitelijkeVerenigingWerdGeregistreerd> @event,
        IDocumentOperations ops)
        => Create(@event, ops, BeheerVerenigingHistoriekProjector.Create);

    public async Task Project(IEvent<AfdelingWerdGeregistreerd> @event, IDocumentOperations ops)
    {
        Create(@event, ops, BeheerVerenigingHistoriekProjector.Create);

        if (!string.IsNullOrEmpty(@event.Data.Moedervereniging.VCode))
            await Update(@event.Data.Moedervereniging.VCode, @event, ops,
                         BeheerVerenigingHistoriekProjector.Apply);
    }

    public void Project(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> @event,
        IDocumentOperations ops)
        => Create(@event, ops, BeheerVerenigingHistoriekProjector.Create);

    public async Task Project(IEvent<NaamWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<KorteNaamWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<KorteBeschrijvingWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<StartdatumWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<DoelgroepWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdToegevoegd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdVerwijderd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<ContactgegevenWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<ContactgegevenUitKBOWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<VertegenwoordigerWerdToegevoegd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<VertegenwoordigerWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<VertegenwoordigerWerdVerwijderd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<LocatieWerdToegevoegd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<LocatieWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<LocatieWerdVerwijderd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<ContactgegevenWerdOvergenomenUitKBO> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<ContactgegevenKonNietOvergenomenWordenUitKBO> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<RoepnaamWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<VerenigingWerdGestopt> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<EinddatumWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<VertegenwoordigerWerdOvergenomenUitKBO> vertegenwoordigerWerdOvergenomenUitKbo,
        IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(vertegenwoordigerWerdOvergenomenUitKbo.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(vertegenwoordigerWerdOvergenomenUitKbo, doc);

        ops.Store(doc);
    }

    private static async Task Update<T>(
        IEvent<T> @event,
        IDocumentOperations ops,
        Action<IEvent<T>, BeheerVerenigingHistoriekDocument> action) where T : notnull
        => await Update(@event.StreamKey!, @event, ops, action);

    private static async Task Update<T>(
        string vCode,
        IEvent<T> @event,
        IDocumentOperations ops,
        Action<IEvent<T>, BeheerVerenigingHistoriekDocument> action) where T : notnull
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(vCode))!;

        action(@event, doc);
        BeheerVerenigingHistoriekProjector.UpdateMetadata(@event, doc);

        ops.Store(doc);
    }

    private static void Create<T>(
        IEvent<T> @event,
        IDocumentOperations ops,
        Func<IEvent<T>, BeheerVerenigingHistoriekDocument> action) where T : notnull
    {
        var doc = action(@event);
        BeheerVerenigingHistoriekProjector.UpdateMetadata(@event, doc);
        ops.Insert(doc);
    }
}
