namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.WijzigBankrekeningnummer.RequestModels;

using System.Runtime.Serialization;

/// <summary>Een bankrekeningnummer van een vereniging</summary>
[DataContract]
public class TeWijzigenBankrekeningnummer
{
    /// <summary>Waar deze rekening voor gebruikt wordt</summary>
    [DataMember (Name = "doel")]
    public string? Doel { get; set; }

    /// <summary>De titularis van het bankrekeningnummer</summary>
    [DataMember (Name = "titularis")]
    public string? Titularis { get; set; }
}
