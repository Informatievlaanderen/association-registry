﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AssociationRegistry.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AssociationRegistry.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Achternaam mag geen nummers bevatten..
        /// </summary>
        public static string AchternaamBevatNummers {
            get {
                return ResourceManager.GetString("AchternaamBevatNummers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Achternaam moet een letter bevatten..
        /// </summary>
        public static string AchternaamZonderLetters {
            get {
                return ResourceManager.GetString("AchternaamZonderLetters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dit adres is niet meer actief in het adressenregister..
        /// </summary>
        public static string AdresInactief {
            get {
                return ResourceManager.GetString("AdresInactief", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Adres kon niet gevalideerd worden bij adressenregister..
        /// </summary>
        public static string AdresKonNietGevalideerdWordenBijAdressenregister {
            get {
                return ResourceManager.GetString("AdresKonNietGevalideerdWordenBijAdressenregister", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Adressenregister niet bereikbaar..
        /// </summary>
        public static string AdresKonNietOvergenomenWorden {
            get {
                return ResourceManager.GetString("AdresKonNietOvergenomenWorden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Foutieve request..
        /// </summary>
        public static string AdresKonNietOvergenomenWordenBadRequest {
            get {
                return ResourceManager.GetString("AdresKonNietOvergenomenWordenBadRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het adres is verwijderd uit het adressenregister..
        /// </summary>
        public static string AdresVerwijderd {
            get {
                return ResourceManager.GetString("AdresVerwijderd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contactgegevens die uit KBO werden overgenomen, kunnen niet verwijderd worden..
        /// </summary>
        public static string ContactgegevenFromKboCannotBeRemoved {
            get {
                return ResourceManager.GetString("ContactgegevenFromKboCannotBeRemoved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contactgegevens die uit KBO werden overgenomen, kunnen niet aangepast worden..
        /// </summary>
        public static string ContactgegevenFromKboCannotBeUpdated {
            get {
                return ResourceManager.GetString("ContactgegevenFromKboCannotBeUpdated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Request kon niet correct behandeld worden. Controleer het formaat en probeer het opnieuw..
        /// </summary>
        public static string CouldNotParseRequestException {
            get {
                return ResourceManager.GetString("CouldNotParseRequestException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Minimum en maximum leeftijd moeten tussen 0 en 150 inclusief liggen..
        /// </summary>
        public static string DoelgroepOutOfRange {
            get {
                return ResourceManager.GetString("DoelgroepOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een waarde in de hoofdactiviteitenLijst mag slechts 1 maal voorkomen..
        /// </summary>
        public static string DuplicateHoofdactiviteit {
            get {
                return ResourceManager.GetString("DuplicateHoofdactiviteit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSZ moet uniek zijn binnen de vereniging..
        /// </summary>
        public static string DuplicateInszProvided {
            get {
                return ResourceManager.GetString("DuplicateInszProvided", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Locaties moeten uniek zijn binnen de vereniging..
        /// </summary>
        public static string DuplicateLocatie {
            get {
                return ResourceManager.GetString("DuplicateLocatie", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een waarde in de werkingsgebieden lijst mag slechts 1 maal voorkomen..
        /// </summary>
        public static string DuplicateWerkingsgebied {
            get {
                return ResourceManager.GetString("DuplicateWerkingsgebied", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Einddatum moet na startdatum liggen..
        /// </summary>
        public static string EinddatumIsBeforeStartdatum {
            get {
                return ResourceManager.GetString("EinddatumIsBeforeStartdatum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Einddatum mag niet in de toekomst liggen..
        /// </summary>
        public static string EinddatumIsInFuture {
            get {
                return ResourceManager.GetString("EinddatumIsInFuture", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De naam van de vereniging is verplicht..
        /// </summary>
        public static string EmptyVerenigingsNaam {
            get {
                return ResourceManager.GetString("EmptyVerenigingsNaam", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er is een fout opgetreden bij het verwerken van de projectie..
        /// </summary>
        public static string FoutBijProjecteren {
            get {
                return ResourceManager.GetString("FoutBijProjecteren", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er werd voor dit KBO-nummer geen geldige vereniging gevonden..
        /// </summary>
        public static string GeenGeldigeVerenigingInKbo {
            get {
                return ResourceManager.GetString("GeenGeldigeVerenigingInKbo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een adres moet bestaan uit straatnaam, huisnummer, postcode, gemeente en land..
        /// </summary>
        public static string IncompleteAdres {
            get {
                return ResourceManager.GetString("IncompleteAdres", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een adresId moet een adresbron en waarde bevatten..
        /// </summary>
        public static string IncompleteAdresId {
            get {
                return ResourceManager.GetString("IncompleteAdresId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het bevestigingstoken is niet geldig voor deze request..
        /// </summary>
        public static string InvalidBevestigingstokenProvided {
            get {
                return ResourceManager.GetString("InvalidBevestigingstokenProvided", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De broncode voor dit adres is niet gekend..
        /// </summary>
        public static string InvalidBroncode {
            get {
                return ResourceManager.GetString("InvalidBroncode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De bronwaarde voor een adres uit het addressenregister moet een Data Vlaanderen PURI zijn..
        /// </summary>
        public static string InvalidBronwaardeForAR {
            get {
                return ResourceManager.GetString("InvalidBronwaardeForAR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het opgegeven contacttype werd niet herkend. (&apos;email&apos;, &apos;website&apos;, &apos;socialmedia&apos;, &apos;telefoon&apos;).
        /// </summary>
        public static string InvalidContactType {
            get {
                return ResourceManager.GetString("InvalidContactType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Datum moet van het formaat &apos;yyyy-MM-dd&apos; zijn..
        /// </summary>
        public static string InvalidDateFormat {
            get {
                return ResourceManager.GetString("InvalidDateFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Minimum leeftijd moet kleiner of gelijk zijn aan maximum leeftijd..
        /// </summary>
        public static string InvalidDoelgroepRange {
            get {
                return ResourceManager.GetString("InvalidDoelgroepRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to E-mail voldoet niet aan het verwachte formaat (naam@domein.vlaanderen). In naam worden de volgende tekens
        ///      toegestaan &apos;!#$%&amp;&apos;*+/=?^_`{|}~-&apos;, in domein enkel &apos;.&apos; en &apos;-&apos;.
        ///    .
        /// </summary>
        public static string InvalidEmailFormat {
            get {
                return ResourceManager.GetString("InvalidEmailFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Foutieve tekens in INSZ..
        /// </summary>
        public static string InvalidInszChars {
            get {
                return ResourceManager.GetString("InvalidInszChars", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSZ moet 11 cijfers bevatten..
        /// </summary>
        public static string InvalidInszLength {
            get {
                return ResourceManager.GetString("InvalidInszLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Incorrect INSZ: foutieve checksum..
        /// </summary>
        public static string InvalidInszMod97 {
            get {
                return ResourceManager.GetString("InvalidInszMod97", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Foutieve tekens in Kbo nummer..
        /// </summary>
        public static string InvalidKboNummerChars {
            get {
                return ResourceManager.GetString("InvalidKboNummerChars", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kbo nummer moet 10 cijfers bevatten..
        /// </summary>
        public static string InvalidKboNummerLength {
            get {
                return ResourceManager.GetString("InvalidKboNummerLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Incorrect Kbo nummer: foutieve checksum..
        /// </summary>
        public static string InvalidKboNummerMod97 {
            get {
                return ResourceManager.GetString("InvalidKboNummerMod97", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Social media url moet beginnen met &apos;http://&apos; of &apos;https://&apos;.
        /// </summary>
        public static string InvalidSocialMediaStart {
            get {
                return ResourceManager.GetString("InvalidSocialMediaStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TelefoonNummer moet bestaan uit cijfers, whitespace en &quot;. / ( ) - + &quot;.
        /// </summary>
        public static string InvalidTelefoonNummerCharacter {
            get {
                return ResourceManager.GetString("InvalidTelefoonNummerCharacter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Formaat van de VCode moet &apos;V0000000&apos; zijn.
        /// </summary>
        public static string InvalidVCodeFormat {
            get {
                return ResourceManager.GetString("InvalidVCodeFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Website url moet beginnen met &apos;http://&apos; of &apos;https://&apos;.
        /// </summary>
        public static string InvalidWebsiteStart {
            get {
                return ResourceManager.GetString("InvalidWebsiteStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De vereniging moet minstens 1 hoofdactiviteit hebben. Je kan de laatste hoofdactiviteit niet verwijderen..
        /// </summary>
        public static string LaatsteHoofdActiviteitKanNietVerwijderdWorden {
            get {
                return ResourceManager.GetString("LaatsteHoofdActiviteitKanNietVerwijderdWorden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De vereniging moet minstens 1 locatie hebben. Je kan de laatste locatie niet verwijderen..
        /// </summary>
        public static string LaatsteLocatieKanNietVerwijderdWorden {
            get {
                return ResourceManager.GetString("LaatsteLocatieKanNietVerwijderdWorden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De vereniging moet minstens 1 vertegenwoordiger hebben. Je kan de laatste vertegenwoordiger niet verwijderen..
        /// </summary>
        public static string LaatsteVertegenwoordigerKanNietVerwijderdWorden {
            get {
                return ResourceManager.GetString("LaatsteVertegenwoordigerKanNietVerwijderdWorden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De maatschappelijke zetel volgens KBO kan niet verwijderd worden..
        /// </summary>
        public static string MaatschappelijkeZetelCanNotBeRemoved {
            get {
                return ResourceManager.GetString("MaatschappelijkeZetelCanNotBeRemoved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De maatschappelijke zetel volgens KBO kan niet gewijzigd worden..
        /// </summary>
        public static string MaatschappelijkeZetelCanNotBeUpdated {
            get {
                return ResourceManager.GetString("MaatschappelijkeZetelCanNotBeUpdated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Het locatietype &apos;Maatschappelijke zetel volgens KBO&apos; kan niet toegekend worden..
        /// </summary>
        public static string MaatschappelijkeZetelIsNotAllowed {
            get {
                return ResourceManager.GetString("MaatschappelijkeZetelIsNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er heeft zich een fout voorgedaan bij het aanroepen van de Magda RegistreerInschrijvingDienst..
        /// </summary>
        public static string MagdaException {
            get {
                return ResourceManager.GetString("MagdaException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een locatie moet minstens een adresId of een adres bevatten..
        /// </summary>
        public static string MissingAdres {
            get {
                return ResourceManager.GetString("MissingAdres", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er kan maar één correspondentie locatie zijn binnen de vereniging..
        /// </summary>
        public static string MultipleCorrespondentieLocaties {
            get {
                return ResourceManager.GetString("MultipleCorrespondentieLocaties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er kan maar één primaire locatie zijn binnen de vereniging..
        /// </summary>
        public static string MultiplePrimaireLocaties {
            get {
                return ResourceManager.GetString("MultiplePrimaireLocaties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Er mag maar één vertegenwoordiger aangeduid zijn als primair contactpersoon..
        /// </summary>
        public static string MultiplePrimaireVertegenwoordigers {
            get {
                return ResourceManager.GetString("MultiplePrimaireVertegenwoordigers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TelefoonNummer moet minstens één cijfer bevatten.
        /// </summary>
        public static string NoNumbersInTelefoonNummer {
            get {
                return ResourceManager.GetString("NoNumbersInTelefoonNummer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to VCode moet groter zijn dan 1000.
        /// </summary>
        public static string OutOfRangeVCode {
            get {
                return ResourceManager.GetString("OutOfRangeVCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Registreer inschrijving voor KBO-nummer kon niet voltooid worden.
        /// </summary>
        public static string RegistreerInschrijvingKonNietVoltooidWorden {
            get {
                return ResourceManager.GetString("RegistreerInschrijvingKonNietVoltooidWorden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Social media url moet minsens één punt bevatten.
        /// </summary>
        public static string SocialMediaMissingPeriod {
            get {
                return ResourceManager.GetString("SocialMediaMissingPeriod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Startdatum moet voor einddatum liggen..
        /// </summary>
        public static string StartdatumIsAfterEinddatum {
            get {
                return ResourceManager.GetString("StartdatumIsAfterEinddatum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Startdatum mag niet in de toekomst liggen..
        /// </summary>
        public static string StartdatumIsInFuture {
            get {
                return ResourceManager.GetString("StartdatumIsInFuture", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De gevraagde vereniging heeft niet de verwachte sequentiewaarde..
        /// </summary>
        public static string UnexpectedAggregateVersion {
            get {
                return ResourceManager.GetString("UnexpectedAggregateVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSZ is niet gekend.
        /// </summary>
        public static string UnknownInsz {
            get {
                return ResourceManager.GetString("UnknownInsz", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deze waarde bevat niet toegestane tekens..
        /// </summary>
        public static string UnsupportedContent {
            get {
                return ResourceManager.GetString("UnsupportedContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deze actie kan niet uitgevoerd worden op dit contactgegeven..
        /// </summary>
        public static string UnsupportedOperationForContactgegevenBron {
            get {
                return ResourceManager.GetString("UnsupportedOperationForContactgegevenBron", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deze actie kan niet uitgevoerd worden op deze locatie..
        /// </summary>
        public static string UnsupportedOperationForLocatietype {
            get {
                return ResourceManager.GetString("UnsupportedOperationForLocatietype", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deze actie kan niet uitgevoerd worden op dit type vereniging..
        /// </summary>
        public static string UnsupportedOperationForVerenigingstype {
            get {
                return ResourceManager.GetString("UnsupportedOperationForVerenigingstype", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deze vereniging kan niet verwijderd worden..
        /// </summary>
        public static string VerenigingKanNietVerwijderdWorden {
            get {
                return ResourceManager.GetString("VerenigingKanNietVerwijderdWorden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aan een KBO vereniging kunnen geen vertegenwoordigers toegevoegd worden..
        /// </summary>
        public static string VerenigingMetRechtspersoonlijkheidCannotAddVertegenwoordigers {
            get {
                return ResourceManager.GetString("VerenigingMetRechtspersoonlijkheidCannotAddVertegenwoordigers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een vertegenwoordiger van een KBO vereniging kan niet verwijderd worden..
        /// </summary>
        public static string VerenigingMetRechtspersoonlijkheidCannotRemoveVertegenwoordigers {
            get {
                return ResourceManager.GetString("VerenigingMetRechtspersoonlijkheidCannotRemoveVertegenwoordigers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Een KBO-vereniging kan niet gestopt worden..
        /// </summary>
        public static string VerenigingMetRechtspersoonlijkheidKanNietGestoptWorden {
            get {
                return ResourceManager.GetString("VerenigingMetRechtspersoonlijkheidKanNietGestoptWorden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deze vereniging werd verwijderd..
        /// </summary>
        public static string VerenigingWerdVerwijderd {
            get {
                return ResourceManager.GetString("VerenigingWerdVerwijderd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voornaam mag geen nummers bevatten..
        /// </summary>
        public static string VoornaamBevatNummers {
            get {
                return ResourceManager.GetString("VoornaamBevatNummers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Voornaam moet een letter bevatten..
        /// </summary>
        public static string VoornaamZonderLetters {
            get {
                return ResourceManager.GetString("VoornaamZonderLetters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Website url moet minsens één punt bevatten.
        /// </summary>
        public static string WebsiteMissingPeriod {
            get {
                return ResourceManager.GetString("WebsiteMissingPeriod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deze zoekopdracht bevat onbekende sorteervelden en kon niet uitgevoerd worden. Onbekende velden: {0}.
        /// </summary>
        public static string ZoekOpdrachtBevatOnbekendeSorteerVelden {
            get {
                return ResourceManager.GetString("ZoekOpdrachtBevatOnbekendeSorteerVelden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deze zoekopdracht kon niet uitgevoerd worden. Controleer alle parameters en probeer opnieuw..
        /// </summary>
        public static string ZoekOpdrachtWasIncorrect {
            get {
                return ResourceManager.GetString("ZoekOpdrachtWasIncorrect", resourceCulture);
            }
        }
    }
}
