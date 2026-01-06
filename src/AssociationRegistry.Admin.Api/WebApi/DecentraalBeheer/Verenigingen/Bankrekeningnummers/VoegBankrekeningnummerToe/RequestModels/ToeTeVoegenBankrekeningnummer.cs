namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;

using System.Runtime.Serialization;

/// <summary>Een bankrekeningnummer van een vereniging</summary>
[DataContract]
public class ToeTeVoegenBankrekeningnummer
{
    /// <summary>
    /// Dit is de unieke identificatie van een bankrekeningnummer
    /// </summary>
    [DataMember]
    public string IBAN { get; set; } = null!;

    /// <summary>De info waarvoor deze rekening gebruikt wordt</summary>
    [DataMember]
    public string GebruiktVoor { get; set; } = null!;

    /// <summary>De titularis van het bankrekeningnummer</summary>
    [DataMember]
    public string Titularis { get; set; } = null!;
}
