namespace AssociationRegistry.Public.ProjectionHost.Projections.Detail;

using Events;
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
    }

    public void Project(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd, IDocumentOperations ops)
    {
        var feitelijkeVereniging = PubliekVerenigingDetailProjector.Create(feitelijkeVerenigingWerdGeregistreerd);

        ops.Insert(feitelijkeVereniging);
    }

    public void Project(IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd, IDocumentOperations ops)
    {
        var verenigingMetRechtspersoonlijkheid = PubliekVerenigingDetailProjector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        ops.Insert(verenigingMetRechtspersoonlijkheid);
    }

    public async Task Project(IEvent<AfdelingWerdGeregistreerd> afdelingWerdGeregistreerd, IDocumentOperations ops)
    {
        var afdeling = PubliekVerenigingDetailProjector.Create(afdelingWerdGeregistreerd);

        ops.Insert(afdeling);

        if (string.IsNullOrEmpty(afdelingWerdGeregistreerd.Data.Moedervereniging.VCode))
            return;

        var moeder = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(afdelingWerdGeregistreerd.Data.Moedervereniging.VCode))!;

        moeder = PubliekVerenigingDetailProjector.Apply(afdelingWerdGeregistreerd, moeder);

        ops.Store(moeder);
    }

    public async Task Project(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(naamWerdGewijzigd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(naamWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<RoepnaamWerdGewijzigd> roepnaamWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(roepnaamWerdGewijzigd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(roepnaamWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(korteNaamWerdGewijzigd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(korteNaamWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(korteBeschrijvingWerdGewijzigd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(korteBeschrijvingWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(startdatumWerdGewijzigd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(startdatumWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<DoelgroepWerdGewijzigd> doelgroepWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(doelgroepWerdGewijzigd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(doelgroepWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(contactgegevenWerdToegevoegd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdToegevoegd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(contactgegevenWerdGewijzigd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(contactgegevenWerdVerwijderd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdVerwijderd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<LocatieWerdToegevoegd> locatieWerdToegevoegd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(locatieWerdToegevoegd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(locatieWerdToegevoegd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<LocatieWerdGewijzigd> locatieWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(locatieWerdGewijzigd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(locatieWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<LocatieWerdVerwijderd> locatieWerdVerwijderd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(locatieWerdVerwijderd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(locatieWerdVerwijderd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofactiviteitenVerenigingloketWerdenGewijzigd, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(hoofactiviteitenVerenigingloketWerdenGewijzigd.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(hoofactiviteitenVerenigingloketWerdenGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> verenigingWerdUitgeschrevenUitPubliekDatastroom, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(verenigingWerdUitgeschrevenUitPubliekDatastroom.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(verenigingWerdUitgeschrevenUitPubliekDatastroom, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> verenigingWerdIngeschrevenInPubliekeDatastroom, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(verenigingWerdIngeschrevenInPubliekeDatastroom.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(verenigingWerdIngeschrevenInPubliekeDatastroom, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> maatschappelijkeZetelWerdOvergenomenUitKbo, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(maatschappelijkeZetelWerdOvergenomenUitKbo.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(maatschappelijkeZetelWerdOvergenomenUitKbo, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdOvergenomenUitKBO> contactgegevenWerdOvergenomenUitKbo, IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<PubliekVerenigingDetailDocument>(contactgegevenWerdOvergenomenUitKbo.StreamKey!))!;

        PubliekVerenigingDetailProjector.Apply(contactgegevenWerdOvergenomenUitKbo, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VerenigingWerdGestopt> verenigingWerdGestopt, IDocumentOperations ops)
        => ops.HardDelete<PubliekVerenigingDetailDocument>(verenigingWerdGestopt.StreamKey!);
}
