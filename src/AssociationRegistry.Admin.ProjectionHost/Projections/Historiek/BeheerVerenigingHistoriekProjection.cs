namespace AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;

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
        Options.EnableDocumentTrackingByIdentity = true;
        Options.DeleteViewTypeOnTeardown<BeheerVerenigingHistoriekDocument>();
    }

    public BeheerVerenigingHistoriekDocument Create(
        IEvent<FeitelijkeVerenigingWerdGeregistreerd> @event,
        IDocumentOperations ops)
        => DoCreate(@event, ops, BeheerVerenigingHistoriekProjector.Create);

    public BeheerVerenigingHistoriekDocument Create(
        IEvent<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> @event,
        IDocumentOperations ops)
        => DoCreate(@event, ops, BeheerVerenigingHistoriekProjector.Create);

    public BeheerVerenigingHistoriekDocument Create(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> @event,
        IDocumentOperations ops)
        => DoCreate(@event, ops, BeheerVerenigingHistoriekProjector.Create);

    public async Task Project(IEvent<NaamWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<KorteNaamWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<KorteBeschrijvingWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<StartdatumWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<StartdatumWerdGewijzigdInKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<DoelgroepWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<WerkingsgebiedenWerdenNietBepaald> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<WerkingsgebiedenWerdenBepaald> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<WerkingsgebiedenWerdenGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<WerkingsgebiedenWerdenNietVanToepassing> @event, IDocumentOperations ops)
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
        IEvent<ContactgegevenWerdGewijzigdInKbo> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<ContactgegevenWerdVerwijderdUitKBO> @event,
        IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<ContactgegevenWerdInBeheerGenomenDoorKbo> @event,
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

    public async Task Project(IEvent<VerenigingWerdVerwijderd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<NaamWerdGewijzigdInKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<KorteNaamWerdGewijzigdInKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<MaatschappelijkeZetelWerdGewijzigdInKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<RechtsvormWerdGewijzigdInKBO> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<VerenigingWerdGestoptInKBO> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<AdresWerdOvergenomenUitAdressenregister> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<AdresWerdGewijzigdInAdressenregister> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<AdresKonNietOvergenomenWordenUitAdressenregister> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<AdresNietUniekInAdressenregister> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<AdresWerdNietGevondenInAdressenregister> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<AdresWerdOntkoppeldVanAdressenregister> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<LidmaatschapWerdToegevoegd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<LidmaatschapWerdGewijzigd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<LidmaatschapWerdVerwijderd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<VerenigingWerdGemarkeerdAlsDubbelVan> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<VerenigingAanvaarddeDubbeleVereniging> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<MarkeringDubbeleVerengingWerdGecorrigeerd> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<VerenigingAanvaarddeCorrectieDubbeleVereniging> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);
    public async Task Project(IEvent<VerenigingssubtypeWerdTerugGezetNaarNietBepaald> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(IEvent<VerenigingssubtypeWerdVerfijndNaarSubvereniging> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

    public async Task Project(
        IEvent<VertegenwoordigerWerdOvergenomenUitKBO> @event,
        IDocumentOperations ops)
    {
        var doc = (await ops.LoadAsync<BeheerVerenigingHistoriekDocument>(@event.StreamKey!))!;

        BeheerVerenigingHistoriekProjector.Apply(@event, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

 public async Task Project(IEvent<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> @event, IDocumentOperations ops)
        => await Update(@event, ops, BeheerVerenigingHistoriekProjector.Apply);

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

    private static BeheerVerenigingHistoriekDocument DoCreate<T>(
        IEvent<T> @event,
        IDocumentOperations ops,
        Func<IEvent<T>, BeheerVerenigingHistoriekDocument> action) where T : notnull
    {
        var doc = action(@event);
        BeheerVerenigingHistoriekProjector.UpdateMetadata(@event, doc);
        ops.Insert(doc);

        return doc;
    }
}
