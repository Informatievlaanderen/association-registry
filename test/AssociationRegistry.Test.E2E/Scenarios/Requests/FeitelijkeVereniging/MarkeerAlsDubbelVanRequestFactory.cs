namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Verenigingen.Dubbels.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using Alba;
using Framework.ApiSetup;
using Givens.FeitelijkeVereniging;
using Marten.Events;
using System.Net;
using Vereniging;

public class MarkeerAlsDubbelVanRequestFactory : ITestRequestFactory<MarkeerAlsDubbelVanRequest>
{
    private readonly MultipleWerdGeregistreerdScenario _scenario;

    public MarkeerAlsDubbelVanRequestFactory(MultipleWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<RequestResult<MarkeerAlsDubbelVanRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new MarkeerAlsDubbelVanRequest
        {
            IsDubbelVan = _scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
        };

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}/dubbelVan");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<MarkeerAlsDubbelVanRequest>(VCode.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode), request);
    }
}
