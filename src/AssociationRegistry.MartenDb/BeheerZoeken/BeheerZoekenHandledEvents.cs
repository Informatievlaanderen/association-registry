namespace AssociationRegistry.MartenDb.BeheerZoeken;

using Events;
using Events.Enriched;

public class BeheerZoekenHandledEvents
{
    public static Type[] Types =
    [
        typeof(FeitelijkeVerenigingWerdGeregistreerdMetPersoonsgegevens),
        typeof(DoelgroepWerdGewijzigd),
        typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdMetPersoonsgegevens),
        typeof(FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid),
        typeof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd),
        typeof(WerkingsgebiedenWerdenNietBepaald),
        typeof(WerkingsgebiedenWerdenBepaald),
        typeof(WerkingsgebiedenWerdenGewijzigd),
        typeof(WerkingsgebiedenWerdenNietVanToepassing),
        typeof(KorteNaamWerdGewijzigd),
        typeof(LocatieWerdGewijzigd),
        typeof(LocatieWerdToegevoegd),
        typeof(LocatieWerdVerwijderd),
        typeof(MaatschappelijkeZetelVolgensKBOWerdGewijzigd),
        typeof(MaatschappelijkeZetelWerdOvergenomenUitKbo),
        typeof(NaamWerdGewijzigd),
        typeof(RoepnaamWerdGewijzigd),
        typeof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd),
        typeof(VerenigingWerdGestopt),
        typeof(VerenigingWerdIngeschrevenInPubliekeDatastroom),
        typeof(VerenigingWerdUitgeschrevenUitPubliekeDatastroom),
        typeof(VerenigingWerdVerwijderd),
        typeof(NaamWerdGewijzigdInKbo),
        typeof(KorteNaamWerdGewijzigdInKbo),
        typeof(MaatschappelijkeZetelWerdGewijzigdInKbo),
        typeof(MaatschappelijkeZetelWerdVerwijderdUitKbo),
        typeof(RechtsvormWerdGewijzigdInKBO),
        typeof(VerenigingWerdGestoptInKBO),
        typeof(StartdatumWerdGewijzigd),
        typeof(StartdatumWerdGewijzigdInKbo),
        typeof(EinddatumWerdGewijzigd),
        typeof(AdresWerdOvergenomenUitAdressenregister),
        typeof(AdresWerdGewijzigdInAdressenregister),
        typeof(LocatieDuplicaatWerdVerwijderdNaAdresMatch),
        typeof(LidmaatschapWerdToegevoegd),
        typeof(LidmaatschapWerdGewijzigd),
        typeof(LidmaatschapWerdVerwijderd),
        typeof(VerenigingWerdGemarkeerdAlsDubbelVan),
        typeof(VerenigingAanvaarddeDubbeleVereniging),
        typeof(WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt),
        typeof(MarkeringDubbeleVerengingWerdGecorrigeerd),
        typeof(VerenigingAanvaarddeCorrectieDubbeleVereniging),
        typeof(VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging),
        typeof(VerenigingssubtypeWerdTerugGezetNaarNietBepaald),
        typeof(VerenigingssubtypeWerdVerfijndNaarSubvereniging),
        typeof(SubverenigingRelatieWerdGewijzigd),
        typeof(SubverenigingDetailsWerdenGewijzigd),
        typeof(GeotagsWerdenBepaald),
    ];
}
