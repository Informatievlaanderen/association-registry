namespace AssociationRegistry.Test.E2E.When_Erkenning_Werd_Geschorst;

using Admin.Api.WebApi.Verenigingen.Erkenningen.SchorsErkenning.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class ErkenningWerdGeschorstContext : TestContextBase<ErkenningWerdGeregistreerdScenario, SchorsErkenningRequest>
{

    protected override ErkenningWerdGeregistreerdScenario InitializeScenario()
        => new();

    public ErkenningWerdGeschorstContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(ErkenningWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new ErkenningWerdGeschorstFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(SchorsErkenningCollection))]
public class SchorsErkenningCollection : ICollectionFixture<ErkenningWerdGeschorstContext>
{
}
