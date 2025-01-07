namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

/// <summary>Een vertegenwoordiger van een vereniging</summary>
[DataContract]
public class Vertegenwoordiger
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

    /// <summary>
    ///     De unieke identificatie code van deze vertegenwoordiger binnen de vereniging
    /// </summary>
    [DataMember(Name = "VertegenwoordigerId")]
    public int VertegenwoordigerId { get; set; }

    /// <summary>
    ///     Het insz van deze vertegenwoordiger
    /// </summary>
    [DataMember(Name = "Insz")]
    public string Insz { get; set; } = null!;

    /// <summary>Dit is de voornaam van de vertegenwoordiger volgens het rijksregister</summary>
    [DataMember(Name = "Voornaam")]
    public string Voornaam { get; init; } = null!;

    /// <summary>Dit is de achternaam van de vertegenwoordiger volgens het rijksregister</summary>
    [DataMember(Name = "Achternaam")]
    public string Achternaam { get; init; } = null!;

    /// <summary>Dit is de roepnaam van de vertegenwoordiger</summary>
    [DataMember(Name = "Roepnaam")]
    public string? Roepnaam { get; init; }

    /// <summary>Dit is de rol van de vertegenwoordiger binnen de vereniging</summary>
    [DataMember(Name = "Rol")]
    public string? Rol { get; init; }

    /// <summary>
    ///     Dit duidt aan dat dit de unieke primaire contactpersoon is voor alle communicatie met overheidsinstanties
    /// </summary>
    [DataMember(Name = "IsPrimair")]
    public bool PrimairContactpersoon { get; init; }

    /// <summary>Het e-mailadres van de vertegenwoordiger</summary>
    [DataMember(Name = "E-mail")]
    public string Email { get; init; } = null!;

    /// <summary>Het telefoonnummer van de vertegenwoordiger</summary>
    [DataMember(Name = "Telefoon")]
    public string Telefoon { get; init; } = null!;

    /// <summary>Het mobiel nummer van de vertegenwoordiger</summary>
    [DataMember(Name = "Mobiel")]
    public string Mobiel { get; init; } = null!;

    /// <summary>Het socialmedia account van de vertegenwoordiger</summary>
    [DataMember(Name = "SocialMedia")]
    public string SocialMedia { get; init; } = null!;

    /// <summary>de contactgegevens van de vertegenwoordiger</summary>
    [DataMember(Name = "VertegenwoordigerContactgegevens")]
    public VertegenwoordigerContactgegevens VertegenwoordigerContactgegevens { get; init; }

    /// <summary> De bron die deze vertegenwoordiger beheert.
    /// <br />
    ///     Mogelijke waarden:<br />
    ///     - Initiator<br />
    ///     - KBO
    /// </summary>
    [DataMember(Name = "Bron")]
    public string Bron { get; set; } = null!;
}
