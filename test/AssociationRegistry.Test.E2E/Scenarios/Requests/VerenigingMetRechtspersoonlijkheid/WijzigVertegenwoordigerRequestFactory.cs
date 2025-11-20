namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VerenigingMetRechtspersoonlijkheid;

using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.MetRechtspersoonlijkheid;
using System.Net;

public class WijzigVertegenwoordigerRequestFactory : ITestRequestFactory<WijzigVertegenwoordigerRequest>
{
    private readonly VertegenwoordigerWerdOvergenomenUitKBOScenario _scenario;

    public WijzigVertegenwoordigerRequestFactory(VertegenwoordigerWerdOvergenomenUitKBOScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<WijzigVertegenwoordigerRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = new WijzigVertegenwoordigerRequest()
        {
            Vertegenwoordiger = fixture.Create<TeWijzigenVertegenwoordiger>(),
        };

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Patch
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode}/vertegenwoordigers/{_scenario.VertegenwoordigerWerdOvergenomenUitKBO.VertegenwoordigerId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<WijzigVertegenwoordigerRequest>(VCode.Create(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode), request, sequence);
    }
}
