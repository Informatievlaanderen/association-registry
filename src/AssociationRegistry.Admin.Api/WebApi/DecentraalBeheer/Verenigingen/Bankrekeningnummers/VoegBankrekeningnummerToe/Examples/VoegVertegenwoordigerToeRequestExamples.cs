namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class VoegBankrekeningnummerToeRequestExamples : IExamplesProvider<VoegBankrekeningnummerToeRequest>
{
    public VoegBankrekeningnummerToeRequest GetExamples()
        => new()
        {
            Bankrekeningnummer = new ToeTeVoegenBankrekeningnummer
            {
               Iban = "BE68539007547034",
               Doel = "Lidgeld",
               Titularis = "Frodo Baggings",
            },
        };
}
