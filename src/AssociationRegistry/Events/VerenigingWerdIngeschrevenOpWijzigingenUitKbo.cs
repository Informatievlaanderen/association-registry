namespace AssociationRegistry.Events;



public record VerenigingWerdIngeschrevenOpWijzigingenUitKbo(
    string KboNummer) : IEvent
{
}

/*
 * KSZ -Sync
 *       ==> SLECHTS 1x door MIGRATIE EVENT in zelfde TX
 *       ==> Geef alle niet ingeschreven vertegenwoordigers
 *          ==> Verenigingen Geregistreerd + Vertegenwoordigers Toegevoegd - Vertegenwoordigers verwijderd
 *       ==> Outbox.Send(RegistreerInschrijvingVertegenwoordiger)
 *       ==> TX.Commit
 *
 * Outbox Handler
 *      ==> RegistreerInschrijvingVertegenwoordiger(VCode + VertegenwoordigerId)
 *          ==> Load Vereniging met VCode
 *          ==> Kijk of Vertegenwoordiger bestaat
 *          ==> Magda.RegistreerInschrijving
 *              SUCCESS ==> VertegenwoordigerWerdIngeschrevenOpWijzigingenUitKsz
 *              OF
 *              3000{2,3,4} ==> VertegenwoordigerKonNietWordenIngeschrevenOpWijzigingenUitKsz(Reden: bestaat niet)
 *              ANDERE FOUTEN ==> Throw new Exception(); ==> DLQ
 *
 * VertegenwoordigerInschrijvingenProjectie
 *          ==> Verenigingen Geregistreerd (VCode + VertegenwoordigerId + Niet ingeschreven
 *          ==>Vertegenwoordigers Toegevoegd (VCode + VertegenwoordigerId + Niet ingeschreven)
 *          ==> VertegenwoordigerIngeschrevenOpWijzigingenUitKsz (VCode + VertegenwoordigerId + Ingeschreven)
 *          ==> VertegenwoordigerKonNietWordenIngeschrevenOpWijzigingenUitKsz (VCode + VertegenwoordigerId + Kon niet ingeschreven worden)
 *
 *

 * KSZ -Sync 2e keer
   *       ==>
   *       ==> Geef alle niet ingeschreven vertegenwoordigers
   *          ==> Verenigingen Geregistreerd + Vertegenwoordigers Toegevoegd - Vertegenwoordigers verwijderd - VertegenwoordigerIngeschrevenOpWijzigingenUitKsz
   *       ==> Outbox.Send(RegistreerInschrijvingVertegenwoordiger)
   *       ==> TX.Commit
   *
 */
