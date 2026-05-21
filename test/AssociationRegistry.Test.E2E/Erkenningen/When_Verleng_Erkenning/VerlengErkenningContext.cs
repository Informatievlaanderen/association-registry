namespace AssociationRegistry.Test.E2E.Erkenningen.When_Verleng_Erkenning;

using Admin.Api.WebApi.Verenigingen.Erkenningen.VerlengErkenning.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class VerlengErkenningContext : TestContextBase<
    VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario, VerlengErkenningRequest>
{
    protected override VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario
        InitializeScenario()
        => new();

    public VerlengErkenningContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario scenario)
    {
        CommandResult = await new VerlengErkenningRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(VerlengErkenningCollection))]
public class VerlengErkenningCollection : ICollectionFixture<VerlengErkenningContext>
{
}
