namespace AssociationRegistry.Test.E2E.When_SubtypeWerdTerugGezetNaarNogNietBepaald;

using AssociationRegistry.Admin.Api.Verenigingen.Subtype;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;
using AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;

public class ZetSubtypeNaarNogNietBepaaldContext: TestContextBase<WijzigSubtypeRequest>
{
    public VCode VCode => RequestResult.VCode;
    public SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario Scenario { get; }

    public ZetSubtypeNaarNogNietBepaaldContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new();
    }

    public override async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        RequestResult = await new WijzigSubtypeRequestVoorNNBFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
