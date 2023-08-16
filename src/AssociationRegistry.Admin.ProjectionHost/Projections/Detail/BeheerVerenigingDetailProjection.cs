namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using System.Threading.Tasks;
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

    public void Project(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd, IDocumentOperations ops)
    {
        var feitelijkeVereniging = BeheerVerenigingDetailProjector.Create(feitelijkeVerenigingWerdGeregistreerd);

        ops.Insert(feitelijkeVereniging);
    }

    public void Project(IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd, IDocumentOperations ops)
    {
        var verenigingMetRechtspersoonlijkheid = BeheerVerenigingDetailProjector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        ops.Insert(verenigingMetRechtspersoonlijkheid);
    }

    public async Task Project(IEvent<AfdelingWerdGeregistreerd> afdelingWerdGeregistreerd, IDocumentOperations ops)
    {
        var afdeling = BeheerVerenigingDetailProjector.Create(afdelingWerdGeregistreerd);

        ops.Insert(afdeling);

        if (string.IsNullOrEmpty(afdelingWerdGeregistreerd.Data.Moedervereniging.VCode))
            return;

        var moeder = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(afdelingWerdGeregistreerd.Data.Moedervereniging.VCode))!;

        moeder = BeheerVerenigingDetailProjector.Apply(afdelingWerdGeregistreerd, moeder);

        ops.Store(moeder);
    }

    public async Task Project(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = await ops.LoadAsync<BeheerVerenigingDetailDocument>(naamWerdGewijzigd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(naamWerdGewijzigd, doc!);

        ops.Store(doc!);
    }

    public async Task Project(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = await ops.LoadAsync<BeheerVerenigingDetailDocument>(korteNaamWerdGewijzigd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(korteNaamWerdGewijzigd, doc!);

        ops.Store(doc!);
    }

    public async Task Project(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = await ops.LoadAsync<BeheerVerenigingDetailDocument>(korteBeschrijvingWerdGewijzigd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(korteBeschrijvingWerdGewijzigd, doc!);

        ops.Store(doc!);
    }

    public async Task Project(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(startdatumWerdGewijzigd.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(startdatumWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<DoelgroepWerdGewijzigd> doelgroepWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(doelgroepWerdGewijzigd.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(doelgroepWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(contactgegevenWerdToegevoegd.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdToegevoegd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(contactgegevenWerdGewijzigd.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(contactgegevenWerdVerwijderd.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdVerwijderd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofactiviteitenVerenigingloketWerdenGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(hoofactiviteitenVerenigingloketWerdenGewijzigd.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(hoofactiviteitenVerenigingloketWerdenGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(vertegenwoordigerWerdToegevoegd.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(vertegenwoordigerWerdToegevoegd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VertegenwoordigerWerdGewijzigd> vertegenwoordigerWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(vertegenwoordigerWerdGewijzigd.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(vertegenwoordigerWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(vertegenwoordigerWerdVerwijderd.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(vertegenwoordigerWerdVerwijderd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> @event, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(@event.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(@event, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> @event, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(@event.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(@event, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<LocatieWerdToegevoegd> @event, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(@event.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(@event, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<LocatieWerdGewijzigd> @event, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(@event.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(@event, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<LocatieWerdVerwijderd> @event, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(@event.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(@event, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> @event, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(@event.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(@event, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdOvergenomenUitKBO> @event, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(@event.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(@event, doc);

        ops.Store(doc);
    }
    public async Task Project(IEvent<ContactgegevenKonNietOvergenomenWordenUitKBO> @event, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingDetailDocument>(@event.StreamKey!))!;

        BeheerVerenigingDetailProjector.Apply(@event, doc);

        ops.Store(doc);
    }
}
