namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerErkenning.RequestModels;
using Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;

public class CorrigeerErkenningRequestFactory : ITestRequestFactory<CorrigeerErkenningRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly ErkenningWerdGeregistreerdScenario _scenario;

    public CorrigeerErkenningRequestFactory(ErkenningWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<CorrigeerErkenningRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vCode = _scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

        var request = fixture.Create<CorrigeerErkenningRequest>();

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.WithRequestHeader(
                    WellknownHeaderNames.Initiator,
                    _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode
                );

                s.Patch.Json(request, JsonStyle.Mvc)
                    .ToUrl(
                        $"/v1/verenigingen/{vCode}/erkenningen/{_scenario.ErkenningWerdGeregistreerd.ErkenningId}"
                    );

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
                s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
            })
        ).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<CorrigeerErkenningRequest>(VCode.Create(vCode), request, sequence);
    }
}
