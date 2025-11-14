namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using Alba;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.FeitelijkeVereniging;
using System.Net;
using Vereniging;

public class MarkeerAlsDubbelVanRequestFactory : ITestRequestFactory<MarkeerAlsDubbelVanRequest>
{
    private readonly MultipleWerdGeregistreerdScenario _scenario;

    public MarkeerAlsDubbelVanRequestFactory(MultipleWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<MarkeerAlsDubbelVanRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new MarkeerAlsDubbelVanRequest
        {
            IsDubbelVan = _scenario.AndereFeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.VCode,
        };

       var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.WithRequestHeader("Authorization", apiSetup.SuperAdminHttpClient.DefaultRequestHeaders.GetValues("Authorization").First());
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.VCode}/dubbelVan");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<MarkeerAlsDubbelVanRequest>(VCode.Create(_scenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.VCode), request, sequence);
    }
}
