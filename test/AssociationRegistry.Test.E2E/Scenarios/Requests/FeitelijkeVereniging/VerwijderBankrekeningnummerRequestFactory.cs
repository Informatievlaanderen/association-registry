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

public class VerwijderBankrekeningnummerRequestFactory : ITestRequestFactory<NullRequest>
{
    private readonly BankrekeningnummerWerdToegevoegdScenario _scenario;

    public VerwijderBankrekeningnummerRequestFactory(BankrekeningnummerWerdToegevoegdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<NullRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Delete.Url($"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/bankrekeningnummers/{_scenario.BankrekeningnummerWerdToegevoegd.BankrekeningnummerId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<NullRequest>(VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode), new NullRequest(), sequence);
    }
}
