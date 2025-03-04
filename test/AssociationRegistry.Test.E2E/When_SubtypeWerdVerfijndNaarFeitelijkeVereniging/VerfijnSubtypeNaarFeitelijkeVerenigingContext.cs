namespace AssociationRegistry.Test.E2E.When_SubtypeWerdVerfijndNaarFeitelijkeVereniging;

using Admin.Api.Verenigingen.Subtype;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;

public class VerfijnSubtypeNaarFeitelijkeVerenigingContext: TestContextBase<WijzigSubtypeRequest>
{
    public VCode VCode => RequestResult.VCode;
    public MultipleWerdGeregistreerdScenario Scenario { get; }

    public VerfijnSubtypeNaarFeitelijkeVerenigingContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new();
    }

    public override async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        RequestResult = await new WijzigSubtypeRequestVoorVerfijnNaarFvFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
