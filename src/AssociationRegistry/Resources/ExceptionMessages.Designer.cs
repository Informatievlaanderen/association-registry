﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AssociationRegistry {
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
        ///   Looks up a localized string similar to Een afdeling kan niet uit de publieke stroom worden uitgeschreven..
        /// </summary>
        public static string AfdelingCanNotBeUnsubscribedFromPubliekeDatastroom {
            get {
                return ResourceManager.GetString("AfdelingCanNotBeUnsubscribedFromPubliekeDatastroom", resourceCulture);
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
        ///   Looks up a localized string similar to De naam van de vereniging is verplicht..
        /// </summary>
        public static string EmptyVerenigingsNaam {
            get {
                return ResourceManager.GetString("EmptyVerenigingsNaam", resourceCulture);
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
        ///   Looks up a localized string similar to E-mail voldoet niet aan het verwachte formaat (naam@domein.vlaanderen). In naam worden de volgende tekens toegestaan &apos;!#$%&amp;&apos;*+/=?^_`{|}~-&apos;, in domein enkel &apos;.&apos; en &apos;-&apos;..
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
        ///   Looks up a localized string similar to TelefoonNummer moet bestaan uit cijfers, whitespace en \&quot;. /( ) - + \&quot;.
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
        ///   Looks up a localized string similar to Dit locatieType is niet toegestaan bij deze actie..
        /// </summary>
        public static string LocatieTypeIsNotAllowed {
            get {
                return ResourceManager.GetString("LocatieTypeIsNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to De maatschappelijke zetel kan niet worden gewijzigd met deze actie..
        /// </summary>
        public static string MaatschappelijkeZetelCanNotBeUpdated {
            get {
                return ResourceManager.GetString("MaatschappelijkeZetelCanNotBeUpdated", resourceCulture);
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
        ///   Looks up a localized string similar to Social media url moet minsens één punt bevatten.
        /// </summary>
        public static string SocialMediaMissingPeriod {
            get {
                return ResourceManager.GetString("SocialMediaMissingPeriod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Startdatum mag niet in de toekomst liggen..
        /// </summary>
        public static string StardatumIsInFuture {
            get {
                return ResourceManager.GetString("StardatumIsInFuture", resourceCulture);
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
        ///   Looks up a localized string similar to Deze actie kan niet uitgevoerd worden op dit type vereniging..
        /// </summary>
        public static string UnsupportedOperationForVerenigingstype {
            get {
                return ResourceManager.GetString("UnsupportedOperationForVerenigingstype", resourceCulture);
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
    }
}
