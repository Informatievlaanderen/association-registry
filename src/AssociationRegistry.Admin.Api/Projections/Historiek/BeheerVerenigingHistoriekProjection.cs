namespace AssociationRegistry.Admin.Api.Projections.Historiek;

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
    public void Project(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd, IDocumentOperations ops)
    {
        var doc = BeheerVerenigingHistoriekProjector.Create(feitelijkeVerenigingWerdGeregistreerd);

        ops.Insert(doc);
    }

    public async Task Project(IEvent<AfdelingWerdGeregistreerd> afdelingWerdGeregistreerd, IDocumentOperations ops)
    {
        var doc = BeheerVerenigingHistoriekProjector.Create(afdelingWerdGeregistreerd);

        ops.Insert(doc);

        if (string.IsNullOrEmpty(afdelingWerdGeregistreerd.Data.Moedervereniging.VCode))
            return;

        var moeder = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(afdelingWerdGeregistreerd.Data.Moedervereniging.VCode))!;

        BeheerVerenigingHistoriekProjector.Apply(afdelingWerdGeregistreerd, moeder);

        ops.Store(moeder);
    }

    public void Project(IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd, IDocumentOperations ops)
    {
        var doc = BeheerVerenigingHistoriekProjector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        ops.Insert(doc);
    }

    public async Task Project(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(naamWerdGewijzigd.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(naamWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(korteNaamWerdGewijzigd.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(korteNaamWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(korteBeschrijvingWerdGewijzigd.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(korteBeschrijvingWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(startdatumWerdGewijzigd.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(startdatumWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofdactiviteitenVerenigingsloketWerdenGewijzigd,
        IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(hoofdactiviteitenVerenigingsloketWerdenGewijzigd.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(hoofdactiviteitenVerenigingsloketWerdenGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(contactgegevenWerdToegevoegd.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdToegevoegd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(contactgegevenWerdVerwijderd.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdVerwijderd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(contactgegevenWerdGewijzigd.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(contactgegevenWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(vertegenwoordigerWerdToegevoegd.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(vertegenwoordigerWerdToegevoegd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VertegenwoordigerWerdGewijzigd> vertegenwoordigerWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(vertegenwoordigerWerdGewijzigd.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(vertegenwoordigerWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(vertegenwoordigerWerdVerwijderd.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(vertegenwoordigerWerdVerwijderd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> verenigingWerdUitgeschrevenUitPubliekeDatastroom, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(verenigingWerdUitgeschrevenUitPubliekeDatastroom.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(verenigingWerdUitgeschrevenUitPubliekeDatastroom, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> verenigingWerdIngeschrevenInPubliekeDatastroom, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(verenigingWerdIngeschrevenInPubliekeDatastroom.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(verenigingWerdIngeschrevenInPubliekeDatastroom, doc);

        ops.Store(doc);
    }
}
