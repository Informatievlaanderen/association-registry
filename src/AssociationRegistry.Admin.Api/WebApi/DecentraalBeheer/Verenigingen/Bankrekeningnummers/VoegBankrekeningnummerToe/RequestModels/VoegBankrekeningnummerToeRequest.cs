namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;

using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Bankrekeningen;
using System.Runtime.Serialization;

[DataContract]
public record VoegBankrekeningnummerToeRequest
{
    /// <summary>De toe te voegen Bankrekeningnummer</summary>
    [DataMember(Name = "Bankrekeningnummer")]
    public ToeTeVoegenBankrekeningnummer Bankrekeningnummer { get; set; } = null!;

    public VoegBankrekeningnummerToeCommand ToCommand(string vCode)
    => new(VCode.Create(vCode), new ToeTevoegenBankrekeningnummer()
    {
        IBAN = IBanNummer.Create(Bankrekeningnummer.IBAN),
        GebruiktVoor = Bankrekeningnummer.GebruiktVoor,
        Titularis = Bankrekeningnummer.Titularis,
    });
}
