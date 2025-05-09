namespace AssociationRegistry.Test.E2E.When_Wijzig_Locatie;

using Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Framework.ApiSetup;
using Vereniging;
using Marten.Events;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;
using Xunit;

[CollectionDefinition("WijzigLidmaatschapContext")]
public class DatabaseCollection : ICollectionFixture<WijzigLocatieContext>
{
}

public class WijzigLocatieContext: ICollectionFixture<WijzigLocatieContext>, IAsyncLifetime
{
    public const string Name = nameof(WijzigLocatieContext);
    public FullBlownApiSetup ApiSetup { get; }
    private FeitelijkeVerenigingWerdGeregistreerdScenario _werdGeregistreerdScenario;
    public WijzigLocatieRequest Request => CommandResult.Request;
    public VCode VCode => CommandResult.VCode;

    public WijzigLocatieContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public async ValueTask InitializeAsync()
    {
        _werdGeregistreerdScenario = new();

        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);
        CommandResult = await new WijzigLocatieRequestFactory(_werdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }

    public CommandResult<WijzigLocatieRequest> CommandResult { get; set; }

    public async ValueTask DisposeAsync()
    {

    }
}
