namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Infrastructure;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using System.Net;

public class VerwijderVertegenwoordigerRequestFactory : ITestRequestFactory<NullRequest>
{
    private readonly VertegenwoordigerWerdToegevoegdScenario _scenario;

    public VerwijderVertegenwoordigerRequestFactory(VertegenwoordigerWerdToegevoegdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<NullRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Delete
             .Url($"/v1/verenigingen/{_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}/vertegenwoordigers/{_scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<NullRequest>(VCode.Create(_scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode), new NullRequest(), sequence);
    }
}
