namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Verenigingen.Subtype.RequestModels;
using Alba;
using Framework.ApiSetup;
using Givens.FeitelijkeVereniging;
using Marten.Events;
using System.Net;
using Vereniging;

public class WijzigSubtypeRequestVoorNNBFactory : ITestRequestFactory<WijzigSubtypeRequest>
{
    private readonly SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario _scenario;

    public WijzigSubtypeRequestVoorNNBFactory(SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<RequestResult<WijzigSubtypeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new WijzigSubtypeRequest
        {
            Subtype = Subtype.NogNietBepaald.Code,
        };

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.SubtypeWerdVerfijndNaarFeitelijkeVereniging.VCode}/subtype");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<WijzigSubtypeRequest>(VCode.Create(_scenario.SubtypeWerdVerfijndNaarFeitelijkeVereniging.VCode), request);
    }
}
