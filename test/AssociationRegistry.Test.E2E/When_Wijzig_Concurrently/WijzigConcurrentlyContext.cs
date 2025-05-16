namespace AssociationRegistry.Test.E2E.When_Wijzig_Concurrently;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;
using AssociationRegistry.Test.E2E.Scenarios.Requests;
using AssociationRegistry.Vereniging;
using Marten.Events;
using Xunit;

public class WijzigConcurrentlyContext: IAsyncLifetime
{
    public const string Name = nameof(WijzigConcurrentlyContext);
    public FullBlownApiSetup ApiSetup { get; }
    public FeitelijkeVerenigingWerdGeregistreerdScenario WerdGeregistreerdScenario { get; private set; }
    public WijzigLocatieRequest Request => CommandResult.Request;
    public VCode VCode => CommandResult.VCode;

    public WijzigConcurrentlyContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public async ValueTask InitializeAsync()
    {
        WerdGeregistreerdScenario = new();

        await ApiSetup.ExecuteGiven(WerdGeregistreerdScenario);
        CommandResult = await new WijzigLocatieRequestFactory(WerdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }

    public CommandResult<WijzigLocatieRequest> CommandResult { get; set; }

    public async ValueTask DisposeAsync()
    {

    }
}
