namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.WijzigBankrekeningnummer.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class WijzigBankrekeningnummerRequestExamples : IExamplesProvider<WijzigBankrekeningnummerRequest>
{
    public WijzigBankrekeningnummerRequest GetExamples()
        => new()
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer()
            {
               Doel = "Lidgeld",
               Titularis = "Frodo Baggings",
            },
        };
}
