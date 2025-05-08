namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VZER;

using Admin.Api.Infrastructure;
using Alba;
using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.AutoFixture;
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

    public async Task<CommandResult<WijzigSubtypeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var request = new WijzigSubtypeRequest
        {
            Subtype = VerenigingssubtypeCode.Subvereniging.Code,
            AndereVereniging = _scenario.BaseScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            Identificatie = "andere identificatie",
            Beschrijving = "andere beschrijving",
        };

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/subtype");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<WijzigSubtypeRequest>(VCode.Create(_scenario.BaseScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode), request, sequence);
    }
}
