namespace AssociationRegistry.Test.E2E.Erkenningen.When_Wijzig_Erkenning;

using Admin.Api.WebApi.Verenigingen.Erkenningen.WijzigErkenning.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class WijzigErkenningContext
    : TestContextBase<ErkenningWerdGeregistreerdScenario, WijzigErkenningRequest>
{
    protected override ErkenningWerdGeregistreerdScenario InitializeScenario() => new();

    public WijzigErkenningContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(ErkenningWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new WijzigErkenningRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(WijzigErkenningCollection))]
public class WijzigErkenningCollection : ICollectionFixture<WijzigErkenningContext> { }
