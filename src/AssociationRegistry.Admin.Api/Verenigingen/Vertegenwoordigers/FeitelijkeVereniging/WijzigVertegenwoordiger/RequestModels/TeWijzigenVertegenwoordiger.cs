namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger.RequestModels;

using Infrastructure.HtmlValidation;
using System.Runtime.Serialization;

/// <summary>De te wijzigen vertegenwoordiger</summary>
[DataContract]
public class TeWijzigenVertegenwoordiger
{
    /// <summary>Dit is de rol van de vertegenwoordiger binnen de vereniging</summary>
    [DataMember]
    [NoHtml]
    public string? Rol { get; set; }

    /// <summary>Dit is de roepnaam van de vertegenwoordiger</summary>
    [DataMember]
    [NoHtml]
    public string? Roepnaam { get; set; }

    /// <summary>
    ///     Dit duidt aan dat dit de unieke primaire contactpersoon is voor alle communicatie met overheidsinstanties
    /// </summary>
    [DataMember]
    public bool? IsPrimair { get; set; }

    /// <summary>Het e-mailadres van de vertegenwoordiger</summary>
    [DataMember(Name = "E-mail")]
    [NoHtml]
    public string? Email { get; set; }

    /// <summary>Het telefoonnummer van de vertegenwoordiger</summary>
    [DataMember]
    [NoHtml]
    public string? Telefoon { get; set; }

    /// <summary>Het mobiel nummer van de vertegenwoordiger</summary>
    [DataMember]
    [NoHtml]
    public string? Mobiel { get; set; }

    /// <summary>Het socialmedia account van de vertegenwoordiger</summary>
    [DataMember]
    [NoHtml]
    public string? SocialMedia { get; set; }
}
