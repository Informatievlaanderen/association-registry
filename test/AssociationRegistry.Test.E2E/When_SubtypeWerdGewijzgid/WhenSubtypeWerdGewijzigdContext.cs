namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid;

using Admin.Api.Verenigingen.Subtype.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.VZER;
using Vereniging;

public class WhenSubtypeWerdGewijzigdContext: TestContextBase<WijzigSubtypeRequest>
{
    public VCode VCode => RequestResult.VCode;
    public SubtypeWerdVerfijndNaarSubverenigingScenario Scenario { get; }

    public WhenSubtypeWerdGewijzigdContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new();
    }

    public override async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        RequestResult = await new WijzigSubtypeRequestVoorWijzigSubtypeFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
