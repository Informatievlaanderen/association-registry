namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;

using System.Runtime.Serialization;

/// <summary>Een bankrekeningnummer van een vereniging</summary>
[DataContract]
public class ToeTeVoegenBankrekeningnummer
{
    /// <summary>
    /// Dit is de unieke identificatie van een bankrekeningnummer
    /// </summary>
    [DataMember (Name = "iban")]
    public string Iban { get; set; } = null!;

    /// <summary>Waar deze rekening voor gebruikt wordt</summary>
    [DataMember (Name = "doel")]
    public string? Doel { get; set; } = null!;

    /// <summary>De titularis van het bankrekeningnummer</summary>
    [DataMember (Name = "titularis")]
    public string Titularis { get; set; } = null!;
}
