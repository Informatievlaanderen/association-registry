namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.TrekRelatieaanvraagIn.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class TrekRelatieaanvraagInRequestExamples : IExamplesProvider<TrekRelatieaanvraagInRequest>
{
    public TrekRelatieaanvraagInRequest GetExamples() => new() { Reden = "De aanvraag werd per vergissing ingediend." };
}
