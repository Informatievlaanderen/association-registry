namespace AssociationRegistry.Admin.Api.Verenigingen;

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

[DataContract]
public class RegistreerVerenigingRequest
{
    /// <summary>Naam van de vereniging</summary>
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
    [DataMember] public ContactInfo[] Contacten { get; set; } = Array.Empty<ContactInfo>();

    [DataContract]
    public class ContactInfo
    {
        /// <summary>
        /// Een groeperingsved dat beschrijft welke contactgegevens worden opgegeven
        /// </summary>
        [DataMember] public string? Contactnaam { get; set; }
        /// <summary>
        /// Een email adres
        /// </summary>
        [DataMember] public string? Email { get; set; }
        /// <summary>
        ///  Een telefoonnummer
        /// </summary>
        [DataMember] public string? Telefoon { get; set; }
        /// <summary>
        /// Een website link
        /// </summary>
        [DataMember] public string? Website { get; set; }
        /// <summary>
        /// Een socialmedia identifier
        /// </summary>
        [DataMember]
        public string? SocialMedia { get; set; }
    }
}
