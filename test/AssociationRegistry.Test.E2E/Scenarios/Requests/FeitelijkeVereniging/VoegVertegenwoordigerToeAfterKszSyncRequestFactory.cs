namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Common;
using Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;

public class VoegVertegenwoordigerToeAfterKszSyncRequestFactory : ITestRequestFactory<VoegVertegenwoordigerToeRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly VzerWerdGeregistreerdAfterKszSyncScenario _scenario;

    public VoegVertegenwoordigerToeAfterKszSyncRequestFactory(VzerWerdGeregistreerdAfterKszSyncScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<VoegVertegenwoordigerToeRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = new VoegVertegenwoordigerToeRequest
        {
            Vertegenwoordiger = fixture.Create<ToeTeVoegenVertegenwoordiger>(),
        };

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.Post.Json(request, JsonStyle.Mvc)
                    .ToUrl(
                        $"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/vertegenwoordigers"
                    );

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
                s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
            })
        ).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<VoegVertegenwoordigerToeRequest>(
            VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode),
            request,
            sequence
        );
    }
}
