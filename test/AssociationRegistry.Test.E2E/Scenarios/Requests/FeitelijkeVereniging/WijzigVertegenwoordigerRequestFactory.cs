namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Common;
using Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using System.Net;

public class WijzigVertegenwoordigerRequestFactory : ITestRequestFactory<WijzigVertegenwoordigerRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly VertegenwoordigerWerdToegevoegdScenario _scenario;

    public WijzigVertegenwoordigerRequestFactory(VertegenwoordigerWerdToegevoegdScenario scenario)
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
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/vertegenwoordigers/{_scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<WijzigVertegenwoordigerRequest>(VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode), request, sequence);
    }
}
