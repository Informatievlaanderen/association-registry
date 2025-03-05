namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Requests;

public class CorrigeerMarkeringAlsDubbelVanContext: TestContextBase<NullRequest>
{
    public VCode VCode => RequestResult.VCode;
    public VerenigingWerdGemarkeerdAlsDubbelVanScenario Scenario { get; }

    public CorrigeerMarkeringAlsDubbelVanContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new();
    }

    public override async ValueTask InitializeAsync()
    {
    }

    public override async ValueTask Init()    {
        await ApiSetup.ExecuteGiven(Scenario);
        RequestResult = await new CorrigeerMarkeringAlsDubbelVanRequestFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
