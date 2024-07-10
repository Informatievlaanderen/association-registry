namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using Events;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Schema.Detail;

public class BeheerVerenigingDetailMultiProjection : MultiStreamProjection<BeheerVerenigingDetailMultiDocument, string>
{
    public BeheerVerenigingDetailMultiProjection()
    {
        // Needs a batch size of 1, because otherwise if Registered and NameChanged arrive in 1 batch/slice,
        // the newly persisted document from xxxWerdGeregistreerd is not in the
        // Query yet when we handle NaamWerdGewijzigd.
        // see also https://martendb.io/events/projections/event-projections.html#reusing-documents-in-the-same-batch

        Identity<FeitelijkeVerenigingWerdGeregistreerd>(x => x.VCode);
        CreateEvent<IEvent<FeitelijkeVerenigingWerdGeregistreerd>>(BeheerVerenigingDetailMultiProjector.Create);

        Identity<IEvent<LocatieWerdToegevoegd>>(x => x.StreamKey);
        ProjectEvent<IEvent<LocatieWerdToegevoegd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<LocatieWerdGewijzigd>>(x => x.StreamKey);
        ProjectEvent<IEvent<LocatieWerdGewijzigd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<LocatieWerdVerwijderd>>(x => x.StreamKey);
        ProjectEvent<IEvent<LocatieWerdVerwijderd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<KorteNaamWerdGewijzigd>>(x => x.StreamKey);
        ProjectEvent<IEvent<KorteNaamWerdGewijzigd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<KorteBeschrijvingWerdGewijzigd>>(x => x.StreamKey);
        ProjectEvent<IEvent<KorteBeschrijvingWerdGewijzigd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<StartdatumWerdGewijzigd>>(x => x.StreamKey);
        ProjectEvent<IEvent<StartdatumWerdGewijzigd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<DoelgroepWerdGewijzigd>>(x => x.StreamKey);
        ProjectEvent<IEvent<DoelgroepWerdGewijzigd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<StartdatumWerdGewijzigdInKbo>>(x => x.StreamKey);
        ProjectEvent<IEvent<StartdatumWerdGewijzigdInKbo>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<ContactgegevenWerdToegevoegd>>(x => x.StreamKey);
        ProjectEvent<IEvent<ContactgegevenWerdToegevoegd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<ContactgegevenWerdGewijzigd>>(x => x.StreamKey);
        ProjectEvent<IEvent<ContactgegevenWerdGewijzigd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<ContactgegevenWerdVerwijderd>>(x => x.StreamKey);
        ProjectEvent<IEvent<ContactgegevenWerdVerwijderd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>>(x => x.StreamKey);

        ProjectEvent<IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>>(
            (doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<VertegenwoordigerWerdToegevoegd>>(x => x.StreamKey);
        ProjectEvent<IEvent<VertegenwoordigerWerdToegevoegd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<VertegenwoordigerWerdGewijzigd>>(x => x.StreamKey);
        ProjectEvent<IEvent<VertegenwoordigerWerdGewijzigd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<VertegenwoordigerWerdVerwijderd>>(x => x.StreamKey);
        ProjectEvent<IEvent<VertegenwoordigerWerdVerwijderd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom>>(x => x.StreamKey);

        ProjectEvent<IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom>>(
            (doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom>>(x => x.StreamKey);

        ProjectEvent<IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom>>(
            (doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo>>(x => x.StreamKey);
        ProjectEvent<IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd>>(x => x.StreamKey);
        ProjectEvent<IEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<ContactgegevenWerdOvergenomenUitKBO>>(x => x.StreamKey);
        ProjectEvent<IEvent<ContactgegevenWerdOvergenomenUitKBO>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<RoepnaamWerdGewijzigd>>(x => x.StreamKey);
        ProjectEvent<IEvent<RoepnaamWerdGewijzigd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<VerenigingWerdGestopt>>(x => x.StreamKey);
        ProjectEvent<IEvent<VerenigingWerdGestopt>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<EinddatumWerdGewijzigd>>(x => x.StreamKey);
        ProjectEvent<IEvent<EinddatumWerdGewijzigd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<VertegenwoordigerWerdOvergenomenUitKBO>>(x => x.StreamKey);
        ProjectEvent<IEvent<VertegenwoordigerWerdOvergenomenUitKBO>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<ContactgegevenUitKBOWerdGewijzigd>>(x => x.StreamKey);
        ProjectEvent<IEvent<ContactgegevenUitKBOWerdGewijzigd>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        // Identity<IEvent<VerenigingWerdVerwijderd>>(x => x.StreamKey);
        //         ProjectEvent<IEvent<VerenigingWerdVerwijderd>>((doc,e) =>  BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<NaamWerdGewijzigdInKbo>>(x => x.StreamKey);
        ProjectEvent<IEvent<NaamWerdGewijzigdInKbo>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<KorteNaamWerdGewijzigdInKbo>>(x => x.StreamKey);
        ProjectEvent<IEvent<KorteNaamWerdGewijzigdInKbo>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<ContactgegevenWerdGewijzigdInKbo>>(x => x.StreamKey);
        ProjectEvent<IEvent<ContactgegevenWerdGewijzigdInKbo>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<ContactgegevenWerdVerwijderdUitKBO>>(x => x.StreamKey);
        ProjectEvent<IEvent<ContactgegevenWerdVerwijderdUitKBO>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<ContactgegevenWerdInBeheerGenomenDoorKbo>>(x => x.StreamKey);
        ProjectEvent<IEvent<ContactgegevenWerdInBeheerGenomenDoorKbo>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<MaatschappelijkeZetelWerdGewijzigdInKbo>>(x => x.StreamKey);
        ProjectEvent<IEvent<MaatschappelijkeZetelWerdGewijzigdInKbo>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo>>(x => x.StreamKey);
        ProjectEvent<IEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<RechtsvormWerdGewijzigdInKBO>>(x => x.StreamKey);
        ProjectEvent<IEvent<RechtsvormWerdGewijzigdInKBO>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<VerenigingWerdGestoptInKBO>>(x => x.StreamKey);
        ProjectEvent<IEvent<VerenigingWerdGestoptInKBO>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<AdresWerdOvergenomenUitAdressenregister>>(x => x.StreamKey);
        ProjectEvent<IEvent<AdresWerdOvergenomenUitAdressenregister>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<AdresWerdNietGevondenInAdressenregister>>(x => x.StreamKey);
        ProjectEvent<IEvent<AdresWerdNietGevondenInAdressenregister>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<AdresNietUniekInAdressenregister>>(x => x.StreamKey);
        ProjectEvent<IEvent<AdresNietUniekInAdressenregister>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<AdresKonNietOvergenomenWordenUitAdressenregister>>(x => x.StreamKey);
        ProjectEvent<IEvent<AdresKonNietOvergenomenWordenUitAdressenregister>>(
            (doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<AdresWerdGewijzigdInAdressenregister>>(x => x.StreamKey);
        ProjectEvent<IEvent<AdresWerdGewijzigdInAdressenregister>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<AdresWerdOntkoppeldVanAdressenregister>>(x => x.StreamKey);
        ProjectEvent<IEvent<AdresWerdOntkoppeldVanAdressenregister>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        // Identity<IEvent<SynchronisatieMetKboWasSuccesvol>>(x => x.StreamKey);
        // ProjectEvent<IEvent<SynchronisatieMetKboWasSuccesvol>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));
        //
        // Identity<IEvent<ContactgegevenKonNietOvergenomenWordenUitKBO>>(x => x.StreamKey);
        // ProjectEvent<IEvent<ContactgegevenKonNietOvergenomenWordenUitKBO>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));
        //
        // Identity<IEvent<MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo>>(x => x.StreamKey);
        // ProjectEvent<IEvent<MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo>>(
        //     (doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));
        //
        // Identity<IEvent<VerenigingWerdIngeschrevenOpWijzigingenUitKbo>>(x => x.StreamKey);
        // ProjectEvent<IEvent<VerenigingWerdIngeschrevenOpWijzigingenUitKbo>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Identity<IEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch>>(x => x.StreamKey);
        ProjectEvent<IEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch>>((doc, e) => BeheerVerenigingDetailMultiProjector.Apply(e, doc));

        Options.EnableDocumentTrackingByIdentity = true;
        Options.DeleteViewTypeOnTeardown<BeheerVerenigingDetailMultiDocument>();
    }
}
