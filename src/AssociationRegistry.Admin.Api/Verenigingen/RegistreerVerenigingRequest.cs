namespace AssociationRegistry.Admin.Api.Verenigingen;

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

[DataContract]
public class RegistreerVerenigingRequest
{
    /// <summary>Instantie die de vereniging aanmaakt</summary>
    [DataMember]
    [Required]
    public string Initiator { get; init; } = null!;

    /// <summary>Naam van de vereniging</summary>
    [DataMember]
    [Required]
    public string Naam { get; init; } = null!;

    /// <summary>Korte naam van de vereniging</summary>
    [DataMember]
    public string? KorteNaam { get; init; }

    /// <summary>Korte beschrijving van de vereniging</summary>
    [DataMember]
    public string? KorteBeschrijving { get; init; }

    /// <summary>Datum waarop de vereniging gestart is. Deze datum mag niet later zijn dan vandaag.</summary>
    [DataMember]
    public DateOnly? StartDatum { get; init; }

    /// <summary>Ondernemingsnummer van de vereniging. Formaat '##########', '#### ### ###' en '####.###.###" zijn toegelaten.</summary>
    [DataMember]
    public string? KboNummer { get; init; }

    /// <summary>
    /// De contacten van deze vereniging
    /// </summary>
    [DataMember]
    public ContactInfo[] Contacten { get; set; } = Array.Empty<ContactInfo>();

    /// <summary>Alle locaties waar deze vereiging actief is.</summary>
    [DataMember]
    public Locatie[] Locaties { get; set; } = Array.Empty<Locatie>();

    [DataContract]
    public class ContactInfo
    {
        /// <summary>
        /// Een groeperingsved dat beschrijft welke contactgegevens worden opgegeven
        /// </summary>
        [DataMember]
        public string? Contactnaam { get; set; }

        /// <summary>
        /// Een email adres
        /// </summary>
        [DataMember]
        public string? Email { get; set; }

        /// <summary>
        ///  Een telefoonnummer
        /// </summary>
        [DataMember]
        public string? Telefoon { get; set; }

        /// <summary>
        /// Een website link
        /// </summary>
        [DataMember]
        public string? Website { get; set; }

        /// <summary>
        /// Een socialmedia identifier
        /// </summary>
        [DataMember]
        public string? SocialMedia { get; set; }
    }

    [DataContract]
    public class Locatie
    {
        /// <summary>
        /// Het soort locatie dat beschreven word.<br/>
        ///
        /// Mogelijke waarden:<br/>
        ///   - Activiteiten<br/>
        ///   - Correspondentie - Slecht één maal mogelijk<br/>
        /// </summary>
        [DataMember]
        public string LocatieType { get; set; } = null!;

        /// <summary>
        /// Zet true als deze locatie de hoofdlocatie van de verenigign is.<br/>
        /// Maximum één aanduiden.
        /// </summary>
        [DataMember]
        public bool HoofdLocatie { get; set; } = false;

        /// <summary>
        /// Een beschrijvende naam voor de locatie
        /// </summary>
        [DataMember]
        public string? Naam { get; set; }

        /// <summary>
        /// De straat van de locatie
        /// </summary>
        [DataMember]
        public string Straatnaam { get; set; } = null!;

        /// <summary>
        /// He huisnummer van de locatie
        /// </summary>
        [DataMember]
        public string Huisnummer { get; set; } = null!;

        /// <summary>
        /// Het busnummer van de locatie
        /// </summary>
        [DataMember]
        public string? Busnummer { get; set; }

        /// <summary>
        /// De postcode van de locatie
        /// </summary>
        [DataMember]
        public string Postcode { get; set; } = null!;

        /// <summary>
        /// De gemeente van de locatie
        /// </summary>
        [DataMember]
        public string Gemeente { get; set; } = null!;

        /// <summary>
        /// Het land van de locatie
        /// </summary>
        [DataMember]
        public string Land { get; set; } = null!;
    }
}
