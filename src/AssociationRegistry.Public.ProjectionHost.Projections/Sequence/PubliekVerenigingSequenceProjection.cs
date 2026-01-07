namespace AssociationRegistry.Public.ProjectionHost.Projections.Sequence;

using Events;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten.Events.Aggregation;
using Marten.Internal.Sessions;
using Schema.Sequence;
using IEvent = Events.IEvent;

public class PubliekVerenigingSequenceProjection : SingleStreamProjection<PubliekVerenigingSequenceDocument, string>
{
    public PubliekVerenigingSequenceProjection()
    {
        Options.DeleteViewTypeOnTeardown<PubliekVerenigingSequenceDocument>();
    }

    public PubliekVerenigingSequenceDocument Create(IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> e)
        => new()
        {
            Sequence = e.Sequence,
            VCode = e.Data.VCode,
            Version = (int)e.Version,
        };

    public PubliekVerenigingSequenceDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> e)
        => new()
        {
            Sequence = e.Sequence,
            VCode = e.Data.VCode,
            Version = (int)e.Version,
        };

    public PubliekVerenigingSequenceDocument Create(IEvent<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> e)
        => new()
        {
            Sequence = e.Sequence,
            VCode = e.Data.VCode,
            Version = (int)e.Version,
        };

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> e,
        PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<AdresHeeftGeenVerschillenMetAdressenregister> e,
        PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<ContactgegevenKonNietOvergenomenWordenUitKBO> e,
        PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<GeotagsWerdenBepaald> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo> e,
        PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<SynchronisatieMetKboWasSuccesvol> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<VerenigingWerdIngeschrevenOpWijzigingenUitKbo> e,
        PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<VerenigingWerdVerwijderd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<NaamWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<KorteNaamWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<KorteBeschrijvingWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<StartdatumWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<StartdatumWerdGewijzigdInKbo> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<DoelgroepWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<ContactgegevenWerdToegevoegd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<ContactgegevenWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<ContactgegevenWerdVerwijderd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<WerkingsgebiedenWerdenNietBepaald> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<WerkingsgebiedenWerdenBepaald> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<WerkingsgebiedenWerdenGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<WerkingsgebiedenWerdenNietVanToepassing> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<VertegenwoordigerWerdToegevoegd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<VertegenwoordigerWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<VertegenwoordigerWerdVerwijderd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<LocatieWerdToegevoegd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<LocatieWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<LocatieWerdVerwijderd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<ContactgegevenWerdOvergenomenUitKBO> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<RoepnaamWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<VerenigingWerdGestopt> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<EinddatumWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<VertegenwoordigerWerdOvergenomenUitKBO> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<VertegenwoordigerWerdToegevoegdVanuitKBO> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<VertegenwoordigerWerdGewijzigdInKBO> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<VertegenwoordigerWerdVerwijderdUitKBO> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<ContactgegevenUitKBOWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<NaamWerdGewijzigdInKbo> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<KorteNaamWerdGewijzigdInKbo> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<ContactgegevenWerdGewijzigdInKbo> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<ContactgegevenWerdVerwijderdUitKBO> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<ContactgegevenWerdInBeheerGenomenDoorKbo> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<MaatschappelijkeZetelWerdGewijzigdInKbo> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<RechtsvormWerdGewijzigdInKBO> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<VerenigingWerdGestoptInKBO> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<AdresWerdOvergenomenUitAdressenregister> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<AdresWerdNietGevondenInAdressenregister> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<AdresNietUniekInAdressenregister> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<AdresKonNietOvergenomenWordenUitAdressenregister> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<AdresWerdGewijzigdInAdressenregister> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<AdresWerdOntkoppeldVanAdressenregister> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<LidmaatschapWerdToegevoegd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<LidmaatschapWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<LidmaatschapWerdVerwijderd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<VerenigingWerdGemarkeerdAlsDubbelVan> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<VerenigingAanvaarddeDubbeleVereniging> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<MarkeringDubbeleVerengingWerdGecorrigeerd> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<VerenigingAanvaarddeCorrectieDubbeleVereniging> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<VerenigingssubtypeWerdTerugGezetNaarNietBepaald> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<VerenigingssubtypeWerdVerfijndNaarSubvereniging> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<KszSyncHeeftVertegenwoordigerBevestigd> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(
        IEvent<BankrekeningnummerWerdToegevoegd> e,
        PubliekVerenigingSequenceDocument doc) => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<SubverenigingRelatieWerdGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    public PubliekVerenigingSequenceDocument Apply(IEvent<SubverenigingDetailsWerdenGewijzigd> e, PubliekVerenigingSequenceDocument doc)
        => UpdateVersion(e, doc);

    private static PubliekVerenigingSequenceDocument UpdateVersion<T>(IEvent<T> e, PubliekVerenigingSequenceDocument doc)
    {
        doc.Version = (int)e.Version;
        doc.Sequence = (int)e.Sequence;

        return doc;
    }
}
