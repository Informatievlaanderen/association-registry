namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Bankrekeningnummer
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; set; }

    /// <summary>
    /// De unieke identificatie code van dit bankrekeningnummer binnen de vereniging
    /// </summary>
    [DataMember(Name = "BankrekeningnummerId")]
    public int BankrekeningnummerId { get; set; }

    /// <summary>
    /// De IBAN van het bankrekeningnummer
    /// </summary>
    [DataMember(Name = "IBAN")]
    public string Iban { get; set; }

    /// <summary>
    /// Waar deze rekening voor gebruikt wordt
    /// </summary>
    [DataMember(Name = "Doel")]
    public string Doel { get; set; } = null!;

    /// <summary>
    /// De titularis van het bankrekeningnummer
    /// </summary>
    [DataMember(Name = "Titularis")]
    public string Titularis { get; set; } = null!;

    /// <summary>
    /// Of het bankrekeningnummer gevalideerd is
    /// </summary>
    [DataMember(Name = "IsGevalideerd")]
    public bool IsGevalideerd { get; set; }
}
