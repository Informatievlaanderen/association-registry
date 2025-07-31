namespace AssociationRegistry.MartenDb.PubliekZoeken;

using Events;

public class PubliekZoekenHandledEvents
{
    public static Type[] Types =
    [
        typeof(FeitelijkeVerenigingWerdGeregistreerd),
        typeof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd),
        typeof(FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid),
        typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd),
        typeof(NaamWerdGewijzigd),
        typeof(RoepnaamWerdGewijzigd),
        typeof(KorteNaamWerdGewijzigd),
        typeof(KorteBeschrijvingWerdGewijzigd),
        typeof(DoelgroepWerdGewijzigd),
        typeof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd),
        typeof(WerkingsgebiedenWerdenNietBepaald),
        typeof(WerkingsgebiedenWerdenBepaald),
        typeof(WerkingsgebiedenWerdenGewijzigd),
        typeof(WerkingsgebiedenWerdenNietVanToepassing),
        typeof(VerenigingWerdUitgeschrevenUitPubliekeDatastroom),
        typeof(VerenigingWerdIngeschrevenInPubliekeDatastroom),
        typeof(LocatieWerdToegevoegd),
        typeof(LocatieWerdGewijzigd),
        typeof(LocatieWerdVerwijderd),
        typeof(MaatschappelijkeZetelWerdOvergenomenUitKbo),
        typeof(MaatschappelijkeZetelVolgensKBOWerdGewijzigd),
        typeof(MaatschappelijkeZetelWerdGewijzigdInKbo),
        typeof(MaatschappelijkeZetelWerdVerwijderdUitKbo),
        typeof(VerenigingWerdGestopt),
        typeof(VerenigingWerdGestoptInKBO),
        typeof(VerenigingWerdVerwijderd),
        typeof(NaamWerdGewijzigdInKbo),
        typeof(KorteNaamWerdGewijzigdInKbo),
        typeof(RechtsvormWerdGewijzigdInKBO),
        typeof(AdresWerdOvergenomenUitAdressenregister),
        typeof(AdresWerdGewijzigdInAdressenregister),
        typeof(LocatieDuplicaatWerdVerwijderdNaAdresMatch),
        typeof(LidmaatschapWerdToegevoegd),
        typeof(LidmaatschapWerdGewijzigd),
        typeof(LidmaatschapWerdVerwijderd),
        typeof(VerenigingWerdGemarkeerdAlsDubbelVan),
        typeof(WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt),
        typeof(MarkeringDubbeleVerengingWerdGecorrigeerd),
        typeof(VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging),
        typeof(VerenigingssubtypeWerdTerugGezetNaarNietBepaald),
        typeof(VerenigingssubtypeWerdVerfijndNaarSubvereniging),
        typeof(SubverenigingRelatieWerdGewijzigd),
        typeof(SubverenigingDetailsWerdenGewijzigd),
        typeof(GeotagsWerdenBepaald),
    ];
}
