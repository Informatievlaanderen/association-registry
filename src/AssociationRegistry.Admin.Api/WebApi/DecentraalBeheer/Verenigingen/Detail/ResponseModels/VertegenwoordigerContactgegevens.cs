namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class VertegenwoordigerContactgegevens
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

    /// <summary>
    ///     Dit duidt aan dat dit de unieke primaire contactpersoon is voor alle communicatie met overheidsinstanties
    /// </summary>
    [DataMember(Name = "IsPrimair")]
    public bool IsPrimair { get; init; }

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
}
