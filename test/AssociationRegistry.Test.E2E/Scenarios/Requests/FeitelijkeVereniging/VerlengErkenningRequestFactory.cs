namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using System.Net;
using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using Admin.Api.WebApi.Verenigingen.Erkenningen.RegistreerErkenning.RequestModels;
using Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning.RequestModels;
using Admin.Api.WebApi.Verenigingen.Erkenningen.VerlengErkenning.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Primitives;

public class VerlengErkenningRequestFactory : ITestRequestFactory<VerlengErkenningRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;

    public VerlengErkenningRequestFactory(
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario scenario
    )
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<VerlengErkenningRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var newHernieuwingsdatum =
            _scenario.ErkenningWerdGeregistreerd.Hernieuwingsdatum.Value.AddDays(fixture.Create<int>());

        var newEinddatum = _scenario.ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(fixture.Create<int>());

        var request = fixture.Create<VerlengErkenningRequest>() with
        {
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(newHernieuwingsdatum),
            Einddatum = newEinddatum,
        };

        var response = (
            await apiSetup.AdminApiHost.Scenario(s =>
            {
                s.WithRequestHeader(
                    WellknownHeaderNames.Initiator,
                    _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode
                );

                s.Post.Json(request, JsonStyle.Mvc)
                    .ToUrl(
                        $"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/erkenningen/{_scenario.ErkenningWerdGeregistreerd.ErkenningId}/verlengingen"
                    );

                s.StatusCodeShouldBe(HttpStatusCode.Accepted);
                s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
                s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
            })
        ).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<VerlengErkenningRequest>(
            VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode),
            request,
            sequence
        );
    }
}
