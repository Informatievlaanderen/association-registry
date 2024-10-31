namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap;

using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;

public class VerwijderLidmaatschapContext: TestContextBase<NullRequest>
{
    public VCode VCode => RequestResult.VCode;
    public LidmaatschapWerdToegevoegdScenario Scenario { get; }

    public VerwijderLidmaatschapContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new(new MultipleWerdGeregistreerdScenario());
    }

    public override async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        RequestResult = await new VerwijderLidmaatschapRequestFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }
}
