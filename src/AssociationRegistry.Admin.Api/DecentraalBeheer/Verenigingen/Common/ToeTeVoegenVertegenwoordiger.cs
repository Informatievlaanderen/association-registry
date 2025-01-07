namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.TelefoonNummers;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

/// <summary>Een vertegenwoordiger van een vereniging</summary>
[DataContract]
public class ToeTeVoegenVertegenwoordiger
{
    /// <summary>
    /// Dit is de unieke identificatie van een vertegenwoordiger, dit kan een rijksregisternummer of bisnummer zijn
    /// </summary>
    [DataMember]
    public string Insz { get; set; } = null!;

    /// <summary>De voornaam van de vertegenwoordiger</summary>
    [DataMember]
    public string Voornaam { get; set; } = null!;

    /// <summary>De achternaam van de vertegenwoordiger</summary>
    [DataMember]
    public string Achternaam { get; set; } = null!;

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
    public bool IsPrimair { get; set; }

    /// <summary>Het e-mailadres van de vertegenwoordiger</summary>
    [DataMember(Name = "E-mail")]
    [JsonPropertyName("E-mail")]
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

    public static Vertegenwoordiger Map(ToeTeVoegenVertegenwoordiger vert)
        => Vertegenwoordiger.Create(
            AssociationRegistry.Vereniging.Insz.Create(vert.Insz),
            vert.IsPrimair,
            vert.Roepnaam,
            vert.Rol,
            AssociationRegistry.Vereniging.Voornaam.Create(vert.Voornaam),
            AssociationRegistry.Vereniging.Achternaam.Create(vert.Achternaam),
            AssociationRegistry.Vereniging.Emails.Email.Create(vert.Email),
            TelefoonNummer.Create(vert.Telefoon),
            TelefoonNummer.Create(vert.Mobiel),
            AssociationRegistry.Vereniging.SocialMedias.SocialMedia.Create(vert.SocialMedia)
        );
}
