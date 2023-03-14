namespace AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;

using System.Runtime.Serialization;

[DataContract]
public class ContactInfo
{
    /// <summary>Dit is de verplichte en unieke identificatie van een contactgegeven</summary>
    [DataMember]
    public string Contactnaam { get; set; } = null!;

    /// <summary>
    /// Een e-mailadres<br/>
    /// <br/>
    /// Hier verwachten we het volgende formaat (naam@domein.vlaanderen),<br/>
    /// In naam worden de volgende characters toegestaan '!#$%&'*+/=?^_`{|}~-',<br/>
    /// in domein enkel '.' en '-'
    /// </summary>
    [DataMember]
    public string? Email { get; set; }

    /// <summary>Een telefoonnummer</summary>
    [DataMember]
    public string? Telefoon { get; set; }

    /// <summary>
    /// Een website link<br/>
    /// <br/>
    /// hier verwachten we steeds een volwaardige HTTP hyperlink (https://example.com)
    /// </summary>
    [DataMember]
    public string? Website { get; set; }

    /// <summary>
    /// Een socialmedia link<br/>
    /// <br/>
    /// hier verwachten we steeds een volwaardige HTTP hyperlink (https://example.com)
    /// </summary>
    [DataMember]
    public string? SocialMedia { get; set; }

    /// <summary>Dit duidt aan dat dit het unieke primaire contactgegeven is</summary>
    [DataMember]
    public bool PrimairContactInfo { get; set; } = false;

    public static Vereniging.CommonCommandDataTypes.ContactInfo ToCommandContactInfo(ContactInfo c)
        => new(c.Contactnaam, c.Email, c.Telefoon, c.Website, c.SocialMedia, c.PrimairContactInfo);
}
