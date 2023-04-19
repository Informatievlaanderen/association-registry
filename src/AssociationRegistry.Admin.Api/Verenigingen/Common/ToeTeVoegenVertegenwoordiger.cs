namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using System.Runtime.Serialization;

/// <summary>Een vertegenwoordiger van een vereniging</summary>
[DataContract]
public class ToeTeVoegenVertegenwoordiger
{
    /// <summary>
    ///     Dit is de unieke identificatie van een vertegenwoordiger, dit kan een rijksregisternummer of bisnummer zijn
    /// </summary>
    [DataMember]
    public string? Insz { get; set; }

    /// <summary>Dit is de rol van de vertegenwoordiger binnen de vereniging</summary>
    [DataMember]
    public string? Rol { get; set; }

    /// <summary>Dit is de roepnaam van de vertegenwoordiger</summary>
    [DataMember]
    public string? Roepnaam { get; set; }

    /// <summary>
    ///     Dit duidt aan dat dit de unieke primaire contactpersoon is voor alle communicatie met overheidsinstanties
    /// </summary>
    [DataMember]
    public bool PrimairContactpersoon { get; set; }

    /// <summary>Het emailadres van de vertegenwoordiger</summary>
    [DataMember]
    public string? Email { get; set; }

    /// <summary>Het telefoonnummer van de vertegenwoordiger</summary>
    [DataMember]
    public string? Telefoon { get; set; }

    /// <summary>Het mobiel nummer van de vertegenwoordiger</summary>
    [DataMember]
    public string? Mobiel { get; set; }

    /// <summary>Het socialmedia account van de vertegenwoordiger</summary>
    [DataMember]
    public string? SocialMedia { get; set; }
}
