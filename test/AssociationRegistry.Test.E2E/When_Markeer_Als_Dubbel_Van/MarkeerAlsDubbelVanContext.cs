namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van;

using Admin.Api.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;

public class MarkeerAlsDubbelVanContext: TestContextBase<MarkeerAlsDubbelVanRequest>
{
    public const string Name = "MarkeerAlsDubbelVanContext";
    public VCode VCode => CommandResult.VCode;
    public MultipleWerdGeregistreerdScenario Scenario { get; }

    public MarkeerAlsDubbelVanContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new();
    }

    public override async ValueTask InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        CommandResult = await new MarkeerAlsDubbelVanRequestFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
