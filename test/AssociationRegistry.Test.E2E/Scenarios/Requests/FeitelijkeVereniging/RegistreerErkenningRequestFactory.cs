namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;

public class RegistreerErkenningRequestFactory : ITestRequestFactory<RegistreerErkenningRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public RegistreerErkenningRequestFactory(
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario
    )
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<RegistreerErkenningRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<RegistreerErkenningRequest>() with
        {
            Erkenning = fixture.Create<TeRegistrerenErkenning>() with
            {
                IpdcProductNummer = "9", // see wiremock
            },
        };

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.Post.Json(request, JsonStyle.Mvc)
                    .ToUrl(
                        $"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/erkenningen"
                    );

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
                s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
            })
        ).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<RegistreerErkenningRequest>(
            VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode),
            request,
            sequence
        );
    }
}
