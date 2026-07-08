namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;

using System.Runtime.Serialization;
using DecentraalBeheer.Vereniging.Bankrekeningen;

/// <summary>Een bankrekeningnummer van een vereniging</summary>
[DataContract]
public class ToeTeVoegenBankrekeningnummer
{
    /// <summary>
    /// Dit is de unieke identificatie van een bankrekeningnummer
    /// </summary>
    [DataMember(Name = "iban")]
    public string Iban { get; set; } = null!;

    /// <summary>Waar deze rekening voor gebruikt wordt</summary>
    [DataMember(Name = "doel")]
    public string? Doel { get; set; }

    /// <summary>De titularis(sen) van het bankrekeningnummer</summary>
    [DataMember(Name = "titularissen")]
    public string[] Titularissen { get; set; } = null!;

    public static ToeTevoegenBankrekeningnummer Map(ToeTeVoegenBankrekeningnummer bankrekeningnummer) =>
        new()
        {
            Iban = IbanNummer.Create(bankrekeningnummer.Iban),
            Doel = bankrekeningnummer.Doel ?? string.Empty,
            Titularissen = DecentraalBeheer.Vereniging.Bankrekeningen.Titularissen.Create(
                bankrekeningnummer.Titularissen
            ),
        };
}
