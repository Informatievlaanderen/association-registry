namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VZER;

using Alba;
using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.AutoFixture;
using Marten.Events;
using System.Net;

public class WijzigSubtypeRequestVoorVerfijnNaarSubFactory : ITestRequestFactory<WijzigSubtypeRequest>
{
    private readonly VzerAndKboVerenigingWerdenGeregistreerdScenario _scenario;
    private readonly Fixture _fixture;

    public WijzigSubtypeRequestVoorVerfijnNaarSubFactory(VzerAndKboVerenigingWerdenGeregistreerdScenario scenario)
    {
        _scenario = scenario;
        _fixture = new Fixture().CustomizeAdminApi();
    }

    public async Task<RequestResult<WijzigSubtypeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new WijzigSubtypeRequest
        {
            Subtype = VerenigingssubtypeCode.Subvereniging.Code,
            AndereVereniging = _scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            Beschrijving = _fixture.Create<string>(),
            Identificatie = _fixture.Create<string>(),
        };

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/subtype");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<WijzigSubtypeRequest>(VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode), request);
    }
}
