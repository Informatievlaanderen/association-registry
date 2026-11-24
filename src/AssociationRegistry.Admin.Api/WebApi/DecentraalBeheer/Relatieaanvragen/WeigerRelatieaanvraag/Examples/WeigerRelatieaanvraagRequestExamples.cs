namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.WeigerRelatieaanvraag.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class WeigerRelatieaanvraagRequestExamples : IExamplesProvider<WeigerRelatieaanvraagRequest>
{
    public WeigerRelatieaanvraagRequest GetExamples() => new() { Reden = "Wij zijn geen lid van deze koepel." };
}
