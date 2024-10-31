namespace AssociationRegistry.Test.E2E.When_Maak_Subvereniging;

using Admin.Api.Verenigingen.Subvereniging.MaakSubvereniging.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Scenarios.Requests.VerenigingOfAnyKind;
using Vereniging;

public class MaakSubverenigingContext: TestContextBase<MaakSubverenigingRequest>
{
    public VCode VCode => RequestResult.VCode;
    public MultipleWerdGeregistreerdScenario Scenario { get; }

    public MaakSubverenigingContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        Scenario = new();
    }

    public override async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(Scenario);
        RequestResult = await new MaakSubverenigingRequestFactory(Scenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }
}
