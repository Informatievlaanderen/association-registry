namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VZER;

using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Subtype.RequestModels;
using Alba;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.AutoFixture;
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

    public async Task<CommandResult<WijzigSubtypeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new WijzigSubtypeRequest
        {
            Subtype = VerenigingssubtypeCode.Subvereniging.Code,
            AndereVereniging = _scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            Beschrijving = _fixture.Create<string>(),
            Identificatie = _fixture.Create<string>(),
        };

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/subtype");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<WijzigSubtypeRequest>(VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode), request, sequence);
    }
}
