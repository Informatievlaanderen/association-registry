namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;
using Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;

public class SchorsErkenningRequestFactory : ITestRequestFactory<SchorsErkenningRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;

    public SchorsErkenningRequestFactory(
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario scenario
    )
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<SchorsErkenningRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var request = fixture.Create<SchorsErkenningRequest>();

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.Post.Json(request, JsonStyle.Mvc)
                    .ToUrl(
                        $"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/erkenningen/{_scenario.ErkenningWerdGeregistreerd.ErkenningId}/schorsingen"
                    );

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
                s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
            })
        ).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<SchorsErkenningRequest>(
            VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode),
            request,
            sequence
        );
    }
}
