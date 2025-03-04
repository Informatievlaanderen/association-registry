namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Verenigingen.Subtype;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using Framework.ApiSetup;
using Givens.FeitelijkeVereniging;
using Marten.Events;
using System.Net;
using Vereniging;

public class WijzigSubtypeRequestFactory : ITestRequestFactory<WijzigSubtypeRequest>
{
    private readonly MultipleWerdGeregistreerdScenario _scenario;
    private readonly string _subType;

    public WijzigSubtypeRequestFactory(MultipleWerdGeregistreerdScenario scenario, string subType)
    {
        _scenario = scenario;
        _subType = subType;
    }

    public async Task<RequestResult<WijzigSubtypeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigSubtypeRequest
        {
            Subtype = _subType,
            AndereVereniging = _scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
            Identificatie = fixture.Create<string>(),
            Beschrijving = fixture.Create<string>(),
        };

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}/subtype");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<WijzigSubtypeRequest>(VCode.Create(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode), request);
    }
}
