namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;

using System.Runtime.Serialization;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Bankrekeningen;

[DataContract]
public record VoegBankrekeningnummerToeRequest
{
    /// <summary>Het toe te voegen bankrekeningnummer</summary>
    [DataMember(Name = "Bankrekeningnummer")]
    public ToeTeVoegenBankrekeningnummer Bankrekeningnummer { get; set; } = null!;

    public VoegBankrekeningnummerToeCommand ToCommand(string vCode) =>
        new(
            VCode.Create(vCode),
            new ToeTevoegenBankrekeningnummer()
            {
                Iban = IbanNummer.Create(Bankrekeningnummer.Iban),
                Doel = Bankrekeningnummer.Doel ?? string.Empty,
                Titularissen = Titularissen.Create(Bankrekeningnummer.Titularissen),
            }
        );
}
