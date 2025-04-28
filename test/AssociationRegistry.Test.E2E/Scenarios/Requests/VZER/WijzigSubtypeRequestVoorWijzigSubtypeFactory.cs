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

public class WijzigSubtypeRequestVoorWijzigSubtypeFactory : ITestRequestFactory<WijzigSubtypeRequest>
{
    private readonly SubtypeWerdVerfijndNaarSubverenigingScenario _scenario;
    private readonly Fixture _fixture;

    public WijzigSubtypeRequestVoorWijzigSubtypeFactory(SubtypeWerdVerfijndNaarSubverenigingScenario scenario)
    {
        _scenario = scenario;
        _fixture = new Fixture().CustomizeAdminApi();
    }

    public async Task<RequestResult<WijzigSubtypeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new WijzigSubtypeRequest
        {
            Subtype = VerenigingssubtypeCode.Subvereniging.Code,
            AndereVereniging = _scenario.BaseScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            Identificatie = "andere identificatie",
            Beschrijving = "andere beschrijving",
        };

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/subtype");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<WijzigSubtypeRequest>(VCode.Create(_scenario.BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode), request);
    }
}
