namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.Infrastructure;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Givens.FeitelijkeVereniging;
using System.Net;
using Vereniging;

public class CorrigeerMarkeringAlsDubbelVanRequestFactory : ITestRequestFactory<NullRequest>
{
    private readonly VerenigingWerdGemarkeerdAlsDubbelVanScenario _scenario;

    public CorrigeerMarkeringAlsDubbelVanRequestFactory(VerenigingWerdGemarkeerdAlsDubbelVanScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<NullRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var vCode = _scenario.MultipleWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd.VCode;

       var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.WithRequestHeader("Authorization", apiSetup.SuperAdminHttpClient.DefaultRequestHeaders.GetValues("Authorization").First());
            s.Delete
             .Url($"/v1/verenigingen/{vCode}/dubbelVan");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        })).Context.Response;

        long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());

        return new CommandResult<NullRequest>(VCode.Create(vCode), new NullRequest(), sequence);
    }
}
