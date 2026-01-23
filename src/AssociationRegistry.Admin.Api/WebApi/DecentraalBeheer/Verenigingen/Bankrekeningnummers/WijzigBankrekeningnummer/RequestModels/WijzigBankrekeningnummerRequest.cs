namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.WijzigBankrekeningnummer.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.WijzigBankrekening;
using DecentraalBeheer.Vereniging;

[DataContract]
public record WijzigBankrekeningnummerRequest
{
    /// <summary>De te wijzigen Bankrekeningnummer</summary>
    [DataMember(Name = "Bankrekeningnummer")]
    public TeWijzigenBankrekeningnummer Bankrekeningnummer { get; set; } = null!;

    public WijzigBankrekeningnummerCommand ToCommand(string vCode, int bankrekeningnummerId)
    => new(VCode.Create(vCode), new AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.TeWijzigenBankrekeningnummer()
    {
        BankrekeningnummerId = bankrekeningnummerId,
        Doel = Bankrekeningnummer.Doel,
        Titularis = Bankrekeningnummer.Titularis,
    });
}
