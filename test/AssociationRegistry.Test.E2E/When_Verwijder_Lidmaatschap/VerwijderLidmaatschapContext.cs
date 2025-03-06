namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap;

using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;
using Scenarios.Requests.FeitelijkeVereniging;
using System;
using System.Threading.Tasks;
using Vereniging;

public class VerwijderLidmaatschapContext: TestContextBase<NullRequest>
{
    public VCode VCode => RequestResult.VCode;
    public LidmaatschapWerdToegevoegdScenario Scenario { get; }

    public VerwijderLidmaatschapContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new(new MultipleWerdGeregistreerdScenario());
    }

    public override async ValueTask InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        RequestResult = await new VerwijderLidmaatschapRequestFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
