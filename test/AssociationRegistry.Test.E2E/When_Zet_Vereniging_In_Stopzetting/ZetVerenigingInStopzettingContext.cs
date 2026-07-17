namespace AssociationRegistry.Test.E2E.When_Zet_Vereniging_In_Stopzetting;

using Admin.Api.WebApi.Verenigingen.InStopzetting.RequestModels;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Stop.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;
using AssociationRegistry.Test.E2E.Scenarios.Requests;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(ZetVerenigingInStopzettingCollection))]
public class ZetVerenigingInStopzettingCollection : ICollectionFixture<ZetVerenigingInStopzettingContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

public class ZetVerenigingInStopzettingContext
    : TestContextBase<FeitelijkeVerenigingWerdGeregistreerdScenario, InStopzettingRequest>
{
    protected override FeitelijkeVerenigingWerdGeregistreerdScenario InitializeScenario() => new();

    public ZetVerenigingInStopzettingContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(FeitelijkeVerenigingWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new ZetVerenigingInStopzettingRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
