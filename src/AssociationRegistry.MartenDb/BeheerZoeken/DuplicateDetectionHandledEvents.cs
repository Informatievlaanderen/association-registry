namespace AssociationRegistry.MartenDb.BeheerZoeken;

using Events;

public class DuplicateDetectionHandledEvents
{
    public static Type[] Types =
    [
        typeof(FeitelijkeVerenigingWerdGeregistreerd),
        typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd),
        typeof(FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid),
        typeof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd),
        typeof(KorteNaamWerdGewijzigd),
        typeof(LocatieWerdGewijzigd),
        typeof(LocatieWerdToegevoegd),
        typeof(LocatieWerdVerwijderd),
        typeof(MaatschappelijkeZetelWerdOvergenomenUitKbo),
        typeof(MaatschappelijkeZetelWerdGewijzigdInKbo),
        typeof(MaatschappelijkeZetelWerdVerwijderdUitKbo),
        typeof(NaamWerdGewijzigd),
        typeof(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd),
        typeof(VerenigingWerdGestopt),
        typeof(VerenigingWerdGestoptInKBO),
        typeof(VerenigingWerdVerwijderd),
        typeof(NaamWerdGewijzigdInKbo),
        typeof(KorteNaamWerdGewijzigdInKbo),
        typeof(AdresWerdOvergenomenUitAdressenregister),
        typeof(AdresWerdGewijzigdInAdressenregister),
        typeof(LocatieDuplicaatWerdVerwijderdNaAdresMatch),
        typeof(VerenigingWerdGemarkeerdAlsDubbelVan),
        typeof(WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt),
        typeof(MarkeringDubbeleVerengingWerdGecorrigeerd),
        typeof(VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging),
        typeof(VerenigingssubtypeWerdTerugGezetNaarNietBepaald),
        typeof(VerenigingssubtypeWerdVerfijndNaarSubvereniging),
        typeof(MaatschappelijkeZetelVolgensKBOWerdGewijzigd),
    ];
}


