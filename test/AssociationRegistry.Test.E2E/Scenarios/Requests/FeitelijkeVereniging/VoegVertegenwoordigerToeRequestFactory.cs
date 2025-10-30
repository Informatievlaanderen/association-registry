namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using System.Net;
using Vereniging;

public class VoegVertegenwoordigerToeRequestFactory : ITestRequestFactory<VoegVertegenwoordigerToeRequest>
{
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public VoegVertegenwoordigerToeRequestFactory(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<VoegVertegenwoordigerToeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<VoegVertegenwoordigerToeRequest>();

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/vertegenwoordigers");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());


        return new CommandResult<VoegVertegenwoordigerToeRequest>(VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode), request, sequence);
    }
}
