namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VZER;

using Alba;
using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using Marten.Events;
using System.Net;

public class WijzigSubtypeRequestVoorNietBepaaldFactory : ITestRequestFactory<WijzigSubtypeRequest>
{
    private readonly SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario _scenario;

    public WijzigSubtypeRequestVoorNietBepaaldFactory(SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<RequestResult<WijzigSubtypeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new WijzigSubtypeRequest
        {
            Subtype = VerenigingssubtypeCode.NietBepaald.Code,
        };

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging.VCode}/subtype");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<WijzigSubtypeRequest>(VCode.Create(_scenario.VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging.VCode), request);
    }
}
