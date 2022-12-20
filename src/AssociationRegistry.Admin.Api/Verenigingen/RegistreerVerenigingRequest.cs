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

    [DataMember] public Contact[] Contacten { get; set; } = Array.Empty<Contact>();

    [DataContract]
    public class Contact
    {
        [DataMember] public string Contactnaam { get; set; } = null!;
        [DataMember] public string? Email { get; set; }
        [DataMember] public string? TelefoonNummer { get; set; }
        [DataMember] public string? Website { get; set; }
    }
}
