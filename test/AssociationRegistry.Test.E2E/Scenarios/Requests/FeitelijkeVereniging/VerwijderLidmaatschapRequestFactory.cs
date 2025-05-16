namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Framework.ApiSetup;
using Givens.FeitelijkeVereniging;
using Marten.Events;
using System.Net;
using Vereniging;

public class VerwijderLidmaatschapRequestFactory : ITestRequestFactory<NullRequest>
{
    private readonly LidmaatschapWerdToegevoegdScenario _scenario;

    public VerwijderLidmaatschapRequestFactory(LidmaatschapWerdToegevoegdScenario scenario)
    {
        _scenario = scenario;
    }

    public async Task<CommandResult<NullRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var vCode = _scenario.LidmaatschapWerdToegevoegd.VCode;
        var lidmaatschapLidmaatschapId = _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId;

        await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Delete
             .Url($"/v1/verenigingen/{vCode}/lidmaatschappen/{lidmaatschapLidmaatschapId}");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
        });

        await apiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
        await apiSetup.PublicProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new CommandResult<NullRequest>(VCode.Create(vCode), new NullRequest());
    }
}
