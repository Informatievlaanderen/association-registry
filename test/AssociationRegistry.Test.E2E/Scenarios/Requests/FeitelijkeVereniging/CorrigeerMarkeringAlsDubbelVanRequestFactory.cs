namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Framework.ApiSetup;
using Givens.FeitelijkeVereniging;
using Marten.Events;
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
        var vCode = _scenario.DubbeleVerenging.VCode;

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.WithRequestHeader("Authorization", apiSetup.SuperAdminHttpClient.DefaultRequestHeaders.GetValues("Authorization").First());
            s.Delete
             .Url($"/v1/verenigingen/{vCode}/dubbelVan");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new CommandResult<NullRequest>(VCode.Create(vCode), new NullRequest());
    }
}
