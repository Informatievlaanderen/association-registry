namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe;

using Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;

public class VoegLidmaatschapToeContext: TestContextBase<VoegLidmaatschapToeRequest>
{
    public const string Name = "VoegLidmaatschapToeContext";
    public VCode VCode => RequestResult.VCode;
    public MultipleWerdGeregistreerdScenario Scenario { get; }

    public VoegLidmaatschapToeContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new();
    }

    public override async ValueTask InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        RequestResult = await new VoegLidmaatschapToeRequestFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
