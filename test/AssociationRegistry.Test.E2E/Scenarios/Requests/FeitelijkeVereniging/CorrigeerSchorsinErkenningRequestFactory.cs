namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Erkenningen.CorrigeerSchorsingErkenning.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;

public class CorrigeerSchorsinErkenningRequestFactory : ITestRequestFactory<CorrigeerSchorsingErkenningRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly ErkenningWerdGeschorstScenario _scenario;

    public CorrigeerSchorsinErkenningRequestFactory(ErkenningWerdGeschorstScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<CorrigeerSchorsingErkenningRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vCode = _scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

        var request = fixture.Create<CorrigeerSchorsingErkenningRequest>();

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.Post.Json(request, JsonStyle.Mvc)
                    .ToUrl(
                        $"/v1/verenigingen/{vCode}/erkenningen/{_scenario.ErkenningWerdGeregistreerd.ErkenningId}/schorsingen"
                    );

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
                s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
            })
        ).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<CorrigeerSchorsingErkenningRequest>(VCode.Create(vCode), request, sequence);
    }
}
