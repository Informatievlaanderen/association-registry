namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger.RequestModels;

using System.Runtime.Serialization;

/// <summary>De te wijzigen vertegenwoordiger</summary>
[DataContract]
public class TeWijzigenVertegenwoordiger
{
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
    public bool? IsPrimair { get; set; }

    /// <summary>Het e-mailadres van de vertegenwoordiger</summary>
    [DataMember(Name = "E-mail")]
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
