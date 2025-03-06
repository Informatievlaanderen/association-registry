namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap;

using Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;

public class WijzigLidmaatschapContext: TestContextBase<WijzigLidmaatschapRequest>
{
    public VCode VCode => RequestResult.VCode;
    public LidmaatschapWerdToegevoegdScenario Scenario { get; }

    public WijzigLidmaatschapContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new(new MultipleWerdGeregistreerdScenario());
    }

    public override async ValueTask InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        RequestResult = await new WijzigLidmaatschapRequestFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
