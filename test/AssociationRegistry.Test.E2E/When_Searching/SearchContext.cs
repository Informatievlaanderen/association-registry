namespace AssociationRegistry.Test.E2E.When_Searching;

using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Vereniging;

public class SearchContext: TestContextBase<NullRequest>
{
    public VCode VCode => RequestResult.VCode;
    public SearchScenario Scenario { get; }

    public SearchContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new SearchScenario();
    }

    public override async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
