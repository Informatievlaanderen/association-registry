namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using System.Net;
using Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;

public class RegistreerErkenningRequestFactory : ITestRequestFactory<RegistreerErkenningRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public RegistreerErkenningRequestFactory(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<RegistreerErkenningRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<RegistreerErkenningRequest>();

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl($"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/erkenningen");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<RegistreerErkenningRequest>(VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode), request, sequence);
    }
}
